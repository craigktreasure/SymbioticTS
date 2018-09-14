using System.Collections.Generic;

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
        /// Gets the contents of the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file contents.</returns>
        string GetContents(string fileName);
    }
}