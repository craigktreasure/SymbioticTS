namespace SymbioticTS.Core.IOAbstractions
{
    internal static class IFileSourceExtensions
    {
        /// <summary>
        /// Copies the <see cref="IFileSource"/> to an <see cref="IFileSink"/>.
        /// </summary>
        /// <param name="fileSource">The file source.</param>
        /// <param name="fileSink">The file sink.</param>
        public static void CopyTo(this IFileSource fileSource, IFileSink fileSink)
        {
            foreach (string file in fileSource.Files)
            {
                fileSink.CreateFile(file, fileSource.GetContents(file));
            }
        }
    }
}