using System.Collections.Generic;
using System.Linq;

namespace SymbioticTS
{
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Applies the specified enumerable ensuring non-deferred enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns><see cref="IReadOnlyList{T}"/>.</returns>
        public static IReadOnlyList<T> Apply<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is IReadOnlyList<T> list)
            {
                return list;
            }

            return enumerable.ToArray();
        }
    }
}