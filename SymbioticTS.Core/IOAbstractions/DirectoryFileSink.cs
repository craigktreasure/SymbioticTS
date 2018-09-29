using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymbioticTS.Core.IOAbstractions
{
    internal class DirectoryFileSink : IFileSink, IFileSource
    {
        private readonly string directoryRoot;

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public IEnumerable<string> Files => Directory.EnumerateFiles(this.directoryRoot)
            .Select(Path.GetFileName);

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryFileSink"/> class.
        /// </summary>
        /// <param name="path">The directory path.</param>
        public DirectoryFileSink(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            this.directoryRoot = path;
        }

        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A <see cref="Stream" />.</returns>
        public Stream CreateFile(string fileName)
        {
            return this.CreateFile(fileName, out string _);
        }

        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="filePath">The file path created.</param>
        /// <returns>A <see cref="Stream" />.</returns>
        public Stream CreateFile(string fileName, out string filePath)
        {
            filePath = Path.Combine(this.directoryRoot, fileName);

            return File.Create(filePath);
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file <see cref="Stream" />.</returns>
        /// <exception cref="FileNotFoundException">The file could not be found.</exception>
        public Stream OpenFile(string fileName)
        {
            string path = Path.Combine(this.directoryRoot, fileName);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The file could not be found.", path);
            }

            return File.OpenRead(path);
        }

        /// <summary>
        /// Gets an <see cref="IFileSource" /> representing the contents of the <see cref="IFileSink" />.
        /// </summary>
        /// <returns>An <see cref="IFileSource" />.</returns>
        public IFileSource ToSource()
        {
            return this;
        }
    }
}