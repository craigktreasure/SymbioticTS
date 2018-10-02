using System;
using Xunit;

namespace SymbioticTS.Core.Tests.Extensions
{
    public class TsTypeSymbolExtensionsTests
    {
        [Fact]
        public void UnwrapArrayNull()
        {
            TsTypeSymbol symbol = null;

            Assert.Throws<ArgumentNullException>(() => symbol.UnwrapArray());
        }

        [Fact]
        public void UnwrapArrayForNonArray()
        {
            TsTypeSymbol symbol = TsTypeSymbol.Number;

            TsTypeSymbol unwrapped = symbol.UnwrapArray(out int rank);

            Assert.Equal(symbol, unwrapped);
            Assert.Equal(0, rank);
        }

        [Fact]
        public void UnwrapArrayForRank1()
        {
            TsTypeSymbol expectedSymbol = TsTypeSymbol.Number;
            TsTypeSymbol symbol = TsTypeSymbol.CreateArraySymbol(expectedSymbol);

            TsTypeSymbol unwrapped = symbol.UnwrapArray(out int rank);

            Assert.Equal(expectedSymbol, unwrapped);
            Assert.Equal(1, rank);
        }

        [Fact]
        public void UnwrapArrayForRank2()
        {
            TsTypeSymbol expectedSymbol = TsTypeSymbol.Number;
            TsTypeSymbol symbol = TsTypeSymbol.CreateArraySymbol(expectedSymbol);
            symbol = TsTypeSymbol.CreateArraySymbol(symbol);

            TsTypeSymbol unwrapped = symbol.UnwrapArray(out int rank);

            Assert.Equal(expectedSymbol, unwrapped);
            Assert.Equal(2, rank);
        }
    }
}