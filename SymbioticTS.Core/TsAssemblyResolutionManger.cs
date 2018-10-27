using System;
using System.Collections.Generic;
using System.IO;

namespace SymbioticTS.Core
{
    internal sealed class TsAssemblyResolutionManger : IDisposable
    {
        private readonly AssemblyResolutionManager assemblyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="TsAssemblyResolutionManger"/> class.
        /// </summary>
        /// <param name="assemblyResolver">The assembly resolver.</param>
        private TsAssemblyResolutionManger(AssemblyResolutionManager assemblyResolver)
        {
            this.assemblyResolver = assemblyResolver;
        }

        /// <summary>
        /// Creates a <see cref="TsAssemblyResolutionManger"/> from the specified assembly path and options.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="transformerOptions">The transformer options.</param>
        /// <returns>A <see cref="TsAssemblyResolutionManger"/>.</returns>
        public static TsAssemblyResolutionManger Create(string assemblyPath, SymbioticTransformerOptions transformerOptions)
        {
            return Create(new[] { Path.GetDirectoryName(assemblyPath) }, transformerOptions);
        }

        /// <summary>
        /// Creates a <see cref="TsAssemblyResolutionManger"/> from the specified assembly lookup paths and options.
        /// </summary>
        /// <param name="assemblyLookupPaths">The assembly lookup paths.</param>
        /// <param name="transformerOptions">The transformer options.</param>
        /// <returns>A <see cref="TsAssemblyResolutionManger"/>.</returns>
        public static TsAssemblyResolutionManger Create(IEnumerable<string> assemblyLookupPaths, SymbioticTransformerOptions transformerOptions)
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

            return new TsAssemblyResolutionManger(assemblyResolver);
        }

        public void Dispose()
        {
            this.assemblyResolver.Dispose();
        }
    }
}