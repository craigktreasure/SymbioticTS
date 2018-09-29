using SymbioticTS.Core.IOAbstractions;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SymbioticTS.Core.Tests
{
    internal static class IFileSourceExtensions
    {
        /// <summary>
        /// Asserts the the file sources contain the same file contents.
        /// </summary>
        /// <param name="expected">The file source containing the expected files.</param>
        /// <param name="actual">The file source containing the actual files.</param>
        public static void AssertSameSource(this IFileSource expected, IFileSource actual)
        {
            var expectedFileMap = expected.Files.ToDictionary(f => Path.GetFileName(f), f => f);
            var actualFileMap = actual.Files.ToDictionary(f => Path.GetFileName(f), f => f);

            Assert.Equal(
                expectedFileMap.Keys.OrderBy(k => k, StringComparer.Ordinal),
                actualFileMap.Keys.OrderBy(k => k, StringComparer.Ordinal));

            foreach ((string fileName, string filePath) in expectedFileMap)
            {
                Assert.Equal(expected.GetFileContent(filePath), actual.GetFileContent(actualFileMap[fileName]));
            }
        }
    }
}