using System.Collections.Generic;
using System.IO;

namespace SymbioticTS.Core.IOAbstractions
{
    internal interface IFileSource
    {
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        IEnumerable<string> Files { get; }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file <see cref="Stream"/>.</returns>
        Stream OpenFile(string fileName);
    }
}