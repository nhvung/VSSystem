using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSSystem.Collections.Generic.Extensions
{
    public static class IEnumerableExtension
    {
        public static List<TResult> ToList<T, TResult>(this IEnumerable<T> src, Func<T, TResult> itemSelector, bool distinct = false, IEqualityComparer<TResult> comparer = null)
        {
            var objs = src?.Select(ite => itemSelector.Invoke(ite));
            if (distinct)
            {
                if (comparer != null)
                {
                    objs = objs?.Distinct(comparer);
                }
                else
                {
                    objs = objs?.Distinct();
                }
            }
            return objs?.ToList();
        }
    }
}
