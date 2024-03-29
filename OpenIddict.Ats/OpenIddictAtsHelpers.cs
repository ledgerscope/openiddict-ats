﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos.Table.Protocol;
using OpenIddict.Abstractions;
using OpenIddict.Ats.Models;

namespace Ats.Driver
{
    /// <summary>
    /// Exposes extensions simplifying the integration between OpenIddict and ATS.
    /// </summary>
    internal static class OpenIddictAtsHelpers
    {
        //public static async Task<IEnumerable<TElement>> ExecuteAsync<TElement>(this TableQuery<TElement> tableQuery, CancellationToken ct)
        //{
        //    var nextQuery = tableQuery;
        //    var continuationToken = default(TableContinuationToken);
        //    var results = new List<TElement>();

        //    do
        //    {
        //        //Execute the next query segment async.
        //        var queryResult = await nextQuery.ExecuteSegmentedAsync(continuationToken, ct);

        //        //Set exact results list capacity with result count.
        //        results.Capacity += queryResult.Results.Count;

        //        //Add segment results to results list.
        //        results.AddRange(queryResult.Results);

        //        continuationToken = queryResult.ContinuationToken;

        //        //Continuation token is not null, more records to load.
        //        if (continuationToken != null && tableQuery.TakeCount.HasValue)
        //        {
        //            //Query has a take count, calculate the remaining number of items to load.
        //            var itemsToLoad = tableQuery.TakeCount.Value - results.Count;

        //            //If more items to load, update query take count, or else set next query to null.
        //            nextQuery = itemsToLoad > 0
        //                ? tableQuery.Take<TElement>(itemsToLoad).AsTableQuery()
        //                : null;
        //        }

        //    } while (continuationToken != null && nextQuery != null && !ct.IsCancellationRequested);

        //    return results;
        //}

        //public static async Task<TElement> FirstOrDefaultAsync<TElement>(this TableQuery<TElement> tableQuery, CancellationToken ct)
        //{
        //    var queryResult = await tableQuery.ExecuteSegmentedAsync(default, ct);

        //    return queryResult.Results.FirstOrDefault();
        //}

        public static async ValueTask<long> CountLongAsync(CloudTable ct, TableQuery<DynamicTableEntity> query, CancellationToken cancellationToken)
        {
            long counter = 0;
            var continuationToken = default(TableContinuationToken);

            do
            {
                var results = await ct.ExecuteQuerySegmentedAsync(query, continuationToken, cancellationToken);
                continuationToken = results.ContinuationToken;
                foreach (var record in results)
                {
                    counter++;
                }
            } while (continuationToken != null);

            return counter;
        }

        public static async Task DeleteAsync<T>(CloudTable ct, TableQuery<T> deleteQuery) where T : ITableEntity, new()
        {
            TableContinuationToken? continuationToken = null;

            do
            {
                var tableQueryResult = await ct.ExecuteQuerySegmentedAsync(deleteQuery, continuationToken);

                continuationToken = tableQueryResult.ContinuationToken;

                // Split into chunks of 100 for batching
                var rowsChunked = tableQueryResult
                    .Where(x => x != null)
                    .GroupBy(x => x.PartitionKey);

                // Delete each chunk of 100 in a batch
                foreach (var rowGroup in rowsChunked)
                {
                    TableBatchOperation tableBatchOperation = new TableBatchOperation();

                    foreach (var row in rowGroup)
                    {
                        tableBatchOperation.Add(TableOperation.Delete(row));

                        if (tableBatchOperation.Count >= 100)
                        {
                            await ct.ExecuteBatchAsync(tableBatchOperation);
                            tableBatchOperation = new TableBatchOperation();
                        }
                    }

                    if (tableBatchOperation.Any())
                        await ct.ExecuteBatchAsync(tableBatchOperation);
                }
            }
            while (continuationToken != null);
        }

        public static string CombineFilters(string op, params string[] filters)
        {
            var f = filters.Where(a => a != null).ToArray();
            string filter = f.First();
            foreach (var item in f.Skip(1))
            {
                filter = TableQuery.CombineFilters(filter, op, item);
            }
            return filter;
        }
    }
}
