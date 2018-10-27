using System.Collections.Generic;
using System.IO;

namespace SymbioticTS.Core
{
    internal static class TsAssemblyResolutionManager
    {
        /// <summary>
        /// Creates a <see cref="AssemblyResolutionManager"/> from the specified assembly path and options.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="transformerOptions">The transformer options.</param>
        /// <returns>A <see cref="AssemblyResolutionManager"/>.</returns>
        public static AssemblyResolutionManager Create(string assemblyPath, SymbioticTransformerOptions transformerOptions)
        {
            return Create(new[] { Path.GetDirectoryName(assemblyPath) }, transformerOptions);
        }

        /// <summary>
        /// Creates a <see cref="AssemblyResolutionManager"/> from the specified assembly lookup paths and options.
        /// </summary>
        /// <param name="assemblyLookupPaths">The assembly lookup paths.</param>
        /// <param name="transformerOptions">The transformer options.</param>
        /// <returns>A <see cref="AssemblyResolutionManager"/>.</returns>
        public static AssemblyResolutionManager Create(IEnumerable<string> assemblyLookupPaths, SymbioticTransformerOptions transformerOptions)
        {
            AssemblyResolutionManager assemblyResolver = AssemblyResolutionManager.CreateDefault();

            if (!string.IsNullOrEmpty(transformerOptions.AssemblyReferencesFilePath))
            {
                string[] assemblyPaths = File.ReadAllLines(transformerOptions.AssemblyReferencesFilePath);
                assemblyResolver.AddAssemblyResolver(StaticAssemblyResolver.Create(assemblyPaths));
            }

            foreach (string lookupPath in assemblyLookupPaths)
            {
                assemblyResolver.AddAssemblyResolver(PathAssemblyResolver.Create(Path.GetDirectoryName(lookupPath)));
            }

            return assemblyResolver;
        }
    }
}