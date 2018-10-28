using CommandLine;

namespace SymbioticTS.Cli
{
    internal class ProgramOptions
    {
        /// <summary>
        /// Gets or sets the path to an assembly references file.
        /// </summary>
        [Option("assembly-references-file", HelpText = "The path to an assembly references file used for assembly resolution.")]
        public string AssemblyReferencesFilePath { get; set; }

        /// <summary>
        /// Gets or sets the input assembly path.
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "The path to the assembly to be transformed.")]
        public string InputAssemblyPath { get; set; }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        [Option('o', "output", Required = true, HelpText = "The path to write the transfomed objects.")]
        public string OutputPath { get; set; }
    }
}