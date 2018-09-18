using DiscoveryReferenceProject;
using System;
using System.Collections.Generic;
using Xunit;

namespace SymbioticTS.Core.Tests
{
    public class TsSymbolLoaderTests
    {
        [Fact]
        public void LoadDiscoveryReferenceProject()
        {
            TsDiscoverer discoverer = new TsDiscoverer();

            IReadOnlyList<Type> discoveredTypes = discoverer.DiscoverTypes(typeof(AssemblyClassToken).Assembly);

            IReadOnlyList<TsTypeSymbol> symbols = TsSymbolLoader.Load(discoveredTypes);

            Assert.Equal(discoveredTypes.Count, symbols.Count);
        }

        [Fact]
        public void LoadReferenceProject()
        {
            TsDiscoverer discoverer = new TsDiscoverer();

            IReadOnlyList<Type> discoveredTypes = discoverer.DiscoverTypes(typeof(ReferenceProject.AssemblyClassToken).Assembly);

            IReadOnlyList<TsTypeSymbol> symbols = TsSymbolLoader.Load(discoveredTypes);

            Assert.Equal(discoveredTypes.Count, symbols.Count);
        }
    }
}