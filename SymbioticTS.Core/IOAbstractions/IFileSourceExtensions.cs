using System.IO;

namespace SymbioticTS.Core.IOAbstractions
{
    internal static class IFileSourceExtensions
    {
        /// <summary>
        /// Copies the <see cref="IFileSource"/> to an <see cref="IFileSink"/>.
        /// </summary>
        /// <param name="sourceFileSource">The source file source.</param>
        /// <param name="targetFileSink">The target file sink.</param>
        public static void CopyTo(this IFileSource sourceFileSource, IFileSink targetFileSink)
        {
            foreach (string file in sourceFileSource.Files)
            {
                using (Stream inputStream = sourceFileSource.OpenFile(file))
                using (Stream outputStream = targetFileSink.CreateFile(file))
                {
                    inputStream.CopyTo(outputStream);
                }
            }
        }

        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        /// <param name="fileSource">The file source.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file contents.</returns>
        public static string GetFileContent(this IFileSource fileSource, string fileName)
        {
            using (Stream stream = fileSource.OpenFile(fileName))
            using (StreamReader reader = new StreamReader(stream, true))
            {
                return reader.ReadToEnd();
            }
        }
    }
}