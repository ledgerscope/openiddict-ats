using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenIddict.Ats.Extensions
{
    internal static class CloudTableExtensions
    {
        public static async IAsyncEnumerable<TElement> ExecuteQueryAllAsync<TElement>(this CloudTable ct, TableQuery<TElement> query, [EnumeratorCancellation] CancellationToken cancellationToken) where TElement : ITableEntity, new()
        {
            var continuationToken = default(TableContinuationToken);

            do
            {
                var results = await ct.ExecuteQuerySegmentedAsync(query, continuationToken, cancellationToken);

                continuationToken = results.ContinuationToken;

                foreach (var token in results)
                {
                    yield return token;
                }
            } while (continuationToken != null);
        }
    }
}
