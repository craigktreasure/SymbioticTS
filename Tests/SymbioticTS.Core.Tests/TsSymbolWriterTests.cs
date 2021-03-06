using SymbioticTS.Abstractions;
using SymbioticTS.Core.IOAbstractions;
using SymbioticTS.TestLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SymbioticTS.Core.Tests
{
    public class TsSymbolWriterTests
    {
        private static readonly string testTempPath = Path.Combine(Path.GetTempPath(), "SymbioticTs", "Tests", nameof(TsSymbolWriterTests));

        [Theory]
        [InlineData(typeof(ScenarioReferenceProject.AssemblyClassToken), @"Content\ReferenceProject")]
        [InlineData(typeof(DiscoveryReferenceProject.AssemblyClassToken), @"Content\DiscoveryReferenceProject")]
        public void WriteReferenceProjectSymbols(Type assemblyType, string expectedResourceQualifier)
        {
            TsTypeManager manager = new TsTypeManager();
            MemoryFileSink fileSink = new MemoryFileSink();
            TsSymbolWriter symbolWriter = new TsSymbolWriter(fileSink);

            IReadOnlyList<Type> discoveredTypes = manager.DiscoverTypes(assemblyType.Assembly);
            IReadOnlyList<TsTypeSymbol> symbols = manager.ResolveTypeSymbols(discoveredTypes);
            symbolWriter.WriteSymbols(symbols);

            IFileSource fileSource = AssemblyResourceFileSource.WithResourceQualifier(
                typeof(TsSymbolWriterTests).Assembly, expectedResourceQualifier);

            try
            {
                fileSource.AssertSameSource(fileSink.ToSource());
            }
            catch
            {
                IFileSink debugFileSink = new DirectoryFileSink(Path.Combine(testTempPath, expectedResourceQualifier));
                fileSink.CopyTo(debugFileSink);
                throw;
            }
        }

        [Theory]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoSimpleTestClass))]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoWithDateTimeCollectionTestClass))]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoWithDateTimeTestClass))]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoWithOptionalPropertyTestClass))]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoWithReadOnlyPropertyTestClass))]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoWithTransformClassCollectionProperty))]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoWithTransformClassProperty))]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoWithTransformInterfaceCollectionProperty))]
        [InlineData(typeof(UnitTestReferenceLibrary.DtoWithTransformInterfaceProperty))]
        public void WriteTypeSymbols(Type type)
        {
            string expectedResourceQualifier = $@"Content\UnitTests\{type.Name}";

            TsTypeManager manager = new TsTypeManager();
            MemoryFileSink fileSink = new MemoryFileSink();
            TsSymbolWriter symbolWriter = new TsSymbolWriter(fileSink);

            IReadOnlyList<Type> discoveredTypes = manager.DiscoverTypes(type);
            IReadOnlyList<TsTypeSymbol> symbols = manager.ResolveTypeSymbols(discoveredTypes);
            symbolWriter.WriteSymbols(symbols);

            IFileSource fileSource = AssemblyResourceFileSource.WithResourceQualifier(
                typeof(TsSymbolWriterTests).Assembly, expectedResourceQualifier);

            try
            {
                fileSource.AssertSameSource(fileSink.ToSource());
            }
            catch
            {
                IFileSink debugFileSink = new DirectoryFileSink(Path.Combine(testTempPath, expectedResourceQualifier));
                fileSink.CopyTo(debugFileSink);
                throw;
            }
        }
    }
}