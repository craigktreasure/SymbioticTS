namespace SymbioticTS.Core.IOAbstractions
{
    internal interface IFileSink
    {
        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content">The content of the file.</param>
        void CreateFile(string fileName, string content);

        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content">The content of the file.</param>
        /// <param name="filePath">The file path created.</param>
        void CreateFile(string fileName, string content, out string filePath);

        /// <summary>
        /// Gets an <see cref="IFileSource"/> representing the contents of the <see cref="IFileSink"/>.
        /// </summary>
        /// <returns>An <see cref="IFileSource"/>.</returns>
        IFileSource ToSource();
    }
}