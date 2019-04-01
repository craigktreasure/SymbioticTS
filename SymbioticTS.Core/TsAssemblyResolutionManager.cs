using System.Collections.Generic;
using System.IO;

namespace SymbioticTS.Core
{
    internal static class TsAssemblyResolutionManager
    {
        /// <summary>
        /// Creates an <see cref="IAssemblyResolver"/> from the specified assembly path and options.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="transformerOptions">The transformer options.</param>
        /// <returns>A <see cref="IAssemblyResolver"/>.</returns>
        public static IAssemblyResolver Create(string assemblyPath, SymbioticTransformerOptions transformerOptions)
        {
            return Create(new[] { Path.GetDirectoryName(assemblyPath) }, transformerOptions);
        }

        /// <summary>
        /// Creates an <see cref="IAssemblyResolver"/> from the specified assembly lookup paths and options.
        /// </summary>
        /// <param name="assemblyLookupPaths">The assembly lookup paths.</param>
        /// <param name="transformerOptions">The transformer options.</param>
        /// <returns>A <see cref="IAssemblyResolver"/>.</returns>
        public static IAssemblyResolver Create(IEnumerable<string> assemblyLookupPaths, SymbioticTransformerOptions transformerOptions)
        {
            CompositeAssemblyResolver assemblyResolver = new CompositeAssemblyResolver();

            if (!string.IsNullOrEmpty(transformerOptions.AssemblyReferencesFilePath))
            {
                string[] assemblyPaths = File.ReadAllLines(transformerOptions.AssemblyReferencesFilePath);
                assemblyResolver.AddAssemblyResolver(ReflectionAssemblyResolver.Create(assemblyPaths));
            }

            foreach (string lookupPath in assemblyLookupPaths)
            {
                assemblyResolver.AddAssemblyResolver(ReflectionAssemblyResolver.CreateFromDirectory(lookupPath));
            }

            return assemblyResolver;
        }
    }
}