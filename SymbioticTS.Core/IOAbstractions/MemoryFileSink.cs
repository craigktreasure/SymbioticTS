using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymbioticTS.Core.IOAbstractions
{
    internal class MemoryFileSink : IFileSink, IFileSource
    {
        private readonly Dictionary<string, byte[]> fileContentMap = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public IEnumerable<string> Files => this.fileContentMap.Keys;

        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A <see cref="Stream" />.</returns>
        /// <exception cref="NotImplementedException"></exception>
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
            this.fileContentMap[fileName] = null;
            filePath = fileName;

            return new DisposeActionMemoryStream(s =>
            {
                this.fileContentMap[fileName] = s.ToArray();
            });
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file <see cref="Stream" />.</returns>
        /// <exception cref="FileNotFoundException">The file could not be found.</exception>
        public Stream OpenFile(string fileName)
        {
            if (this.fileContentMap.TryGetValue(fileName, out byte[] buffer))
            {
                if (buffer == null)
                {
                    return new MemoryStream(0);
                }

                return new MemoryStream(buffer);
            }

            throw new FileNotFoundException("The file could not be found.", fileName);
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