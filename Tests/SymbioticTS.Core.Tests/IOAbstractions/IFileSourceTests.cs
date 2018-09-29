using SymbioticTS.Core.IOAbstractions;
using SymbioticTS.TestLibrary;
using System;
using Xunit;

namespace SymbioticTS.Core.Tests.IOAbstractions
{
    public class IFileSourceTests
    {
        [Fact]
        public void CopyToTest()
        {
            IFileSource fileSource = AssemblyResourceFileSource.WithResourceQualifier(typeof(IFileSourceTests).Assembly, @"Content\ReferenceProject");

            IFileSink fileSink = new MemoryFileSink();

            fileSource.CopyTo(fileSink);

            fileSource.AssertSameSource(fileSink.ToSource());
        }
    }
}