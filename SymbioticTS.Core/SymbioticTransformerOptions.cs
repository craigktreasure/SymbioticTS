namespace SymbioticTS.Core
{
    internal class SymbioticTransformerOptions
    {
        /// <summary>
        /// Gets the default options.
        /// </summary>
        public static SymbioticTransformerOptions Default => new SymbioticTransformerOptions();

        /// <summary>
        /// Gets or sets the path to an assembly references file.
        /// </summary>
        public string AssemblyReferencesFilePath { get; set; }
    }
}