using System;
using System.Collections.Generic;
using System.IO;

namespace SymbioticTS.Core.IOAbstractions
{
    internal class MemoryFileSink : IFileSink, IFileSource
    {
        private readonly Dictionary<string, string> fileContentMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public IEnumerable<string> Files => this.fileContentMap.Keys;

        public void CreateFile(string fileName, string content)
        {
            this.CreateFile(fileName, content, out _);
        }

        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content">The content of the file.</param>
        /// <param name="filePath">The file path created.</param>
        public void CreateFile(string fileName, string content, out string filePath)
        {
            this.fileContentMap[fileName] = content;

            filePath = fileName;
        }

        /// <summary>
        /// Gets the contents of the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file contents.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file could not be found.</exception>
        public string GetContents(string fileName)
        {
            if (!this.fileContentMap.TryGetValue(fileName, out string contents))
            {
                throw new FileNotFoundException("The file could not be found.", fileName);
            }

            return contents;
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