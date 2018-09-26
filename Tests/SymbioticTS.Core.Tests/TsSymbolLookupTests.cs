using System;
using System.Collections.Generic;
using Xunit;

namespace SymbioticTS.Core.Tests
{
    public class TsSymbolLookupTests
    {
        private readonly TsSymbolLookup lookup = new TsSymbolLookup();

        [Theory]
        [InlineData(typeof(object), "any")]
        [InlineData(typeof(int), "number")]
        [InlineData(typeof(uint), "number")]
        [InlineData(typeof(short), "number")]
        [InlineData(typeof(ushort), "number")]
        [InlineData(typeof(long), "number")]
        [InlineData(typeof(ulong), "number")]
        [InlineData(typeof(bool), "boolean")]
        [InlineData(typeof(string), "string")]
        [InlineData(typeof(DateTime), "Date")]
        [InlineData(typeof(DateTimeOffset), "Date")]
        [InlineData(typeof(int[]), "number[]")]
        [InlineData(typeof(int[][]), "number[][]")]
        [InlineData(typeof(IEnumerable<int>), "number[]")]
        [InlineData(typeof(ICollection<int>), "number[]")]
        [InlineData(typeof(IList<int>), "number[]")]
        [InlineData(typeof(IReadOnlyCollection<int>), "number[]")]
        [InlineData(typeof(IReadOnlyList<int>), "number[]")]
        [InlineData(typeof(List<int>), "number[]")]
        [InlineData(typeof(Nullable<int>), "number")]
        public void ResolveSymbol(Type type, string expectedSymbolName)
        {
            TsTypeSymbol typeSymbol = this.lookup.ResolveSymbol(type);

            Assert.Equal(expectedSymbolName, typeSymbol.Name);
        }

        [Theory]
        [InlineData(typeof(Array))]
        [InlineData(typeof(Dictionary<int, string>))]
        [InlineData(typeof(HashSet<int>))]
        [InlineData(typeof(int[,]))]
        [InlineData(typeof(IEnumerable<Version>))]
        [InlineData(typeof(ISet<int>))]
        [InlineData(typeof(SortedSet<int>))]
        [InlineData(typeof(Version))]
        public void SymbolResolutionFail(Type type)
        {
            Assert.Throws<KeyNotFoundException>(() => this.lookup.ResolveSymbol(type));
        }
    }
}
