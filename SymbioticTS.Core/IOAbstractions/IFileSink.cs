using System.IO;

namespace SymbioticTS.Core.IOAbstractions
{
    internal interface IFileSink
    {
        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A <see cref="Stream"/>.</returns>
        Stream CreateFile(string fileName);

        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="filePath">The file path created.</param>
        /// <returns>A <see cref="Stream"/>.</returns>
        Stream CreateFile(string fileName, out string filePath);

        /// <summary>
        /// Gets an <see cref="IFileSource"/> representing the contents of the <see cref="IFileSink"/>.
        /// </summary>
        /// <returns>An <see cref="IFileSource"/>.</returns>
        IFileSource ToSource();
    }
}