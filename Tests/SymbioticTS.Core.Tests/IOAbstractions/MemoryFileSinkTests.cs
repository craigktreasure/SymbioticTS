using SymbioticTS.Core.IOAbstractions;
using System.IO;
using Xunit;

namespace SymbioticTS.Core.Tests.IOAbstractions
{
    public class MemoryFileSinkTests
    {
        [Fact]
        public void StreamWriteWithDisposeTest()
        {
            MemoryFileSink fileSink = new MemoryFileSink();

            string fileName = "foo.txt";
            string expectedText = "It works!";

            using (Stream stream = fileSink.CreateFile(fileName))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                Assert.Contains(fileName, fileSink.Files);

                writer.Write(expectedText);

                Assert.Equal(string.Empty, fileSink.GetFileContent(fileName));
            }

            Assert.Equal(expectedText, fileSink.GetFileContent(fileName));
        }
    }
}