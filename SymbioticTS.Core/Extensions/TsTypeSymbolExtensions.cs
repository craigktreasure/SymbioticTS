using System;

namespace SymbioticTS.Core
{
    internal static class TsTypeSymbolExtensions
    {
        /// <summary>
        /// Unwraps an array symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns><see cref="TsTypeSymbol"/>.</returns>
        public static TsTypeSymbol UnwrapArray(this TsTypeSymbol symbol)
        {
            return UnwrapArray(symbol, out _);
        }

        /// <summary>
        /// Unwraps an array symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="rank">The rank.</param>
        /// <returns><see cref="TsTypeSymbol"/>.</returns>
        public static TsTypeSymbol UnwrapArray(this TsTypeSymbol symbol, out int rank)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (symbol.IsArray)
            {
                TsTypeSymbol result = UnwrapArray(symbol.ElementType, out int elementRank);
                rank = elementRank + 1;
                return result;
            }

            rank = 0;
            return symbol;
        }
    }
}