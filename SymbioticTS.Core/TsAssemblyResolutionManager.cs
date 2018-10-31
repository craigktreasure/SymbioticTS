using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;

namespace SymbioticTS.Core
{
    internal static class TsAssemblyResolutionManager
    {
        /// <summary>
        /// Creates a <see cref="AssemblyResolutionManger" /> from the specified assembly path and options.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="transformerOptions">The transformer options.</param>
        /// <param name="loadContext">The assembly load context.</param>
        /// <returns>A <see cref="AssemblyResolutionManger" />.</returns>
        public static AssemblyResolutionManager Create(string assemblyPath, SymbioticTransformerOptions transformerOptions, AssemblyLoadContext loadContext)
        {
            return Create(new[] { Path.GetDirectoryName(assemblyPath) }, transformerOptions, loadContext);
        }

        /// <summary>
        /// Creates a <see cref="AssemblyResolutionManger" /> from the specified assembly lookup paths and options.
        /// </summary>
        /// <param name="assemblyLookupPaths">The assembly lookup paths.</param>
        /// <param name="transformerOptions">The transformer options.</param>
        /// <param name="loadContext">The assembly load context.</param>
        /// <returns>A <see cref="AssemblyResolutionManger" />.</returns>
        public static AssemblyResolutionManager Create(IEnumerable<string> assemblyLookupPaths, SymbioticTransformerOptions transformerOptions, AssemblyLoadContext loadContext)
        {
            AssemblyResolutionManager assemblyResolver = AssemblyResolutionManager.Create(loadContext);

            if (!string.IsNullOrEmpty(transformerOptions.AssemblyReferencesFilePath))
            {
                string[] assemblyPaths = File.ReadAllLines(transformerOptions.AssemblyReferencesFilePath);
                assemblyResolver.AddAssemblyResolver(StaticAssemblyResolver.Create(assemblyPaths, loadContext));
            }

            foreach (string lookupPath in assemblyLookupPaths)
            {
                assemblyResolver.AddAssemblyResolver(PathAssemblyResolver.Create(lookupPath, loadContext));
            }

            return assemblyResolver;
        }
    }
}