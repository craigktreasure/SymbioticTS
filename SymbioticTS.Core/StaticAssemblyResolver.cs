using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace SymbioticTS.Core
{
    internal sealed class StaticAssemblyResolver : IAssemblyResolver
    {
        private readonly IReadOnlyDictionary<string, string> assemblyNameToPathMap;

        private readonly AssemblyLoadContext loadContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticAssemblyResolver" /> class.
        /// </summary>
        /// <param name="assemblyNameToPathMap">The assembly name to path map.</param>
        /// <param name="loadContext">The assembly load context.</param>
        private StaticAssemblyResolver(IReadOnlyDictionary<string, string> assemblyNameToPathMap, AssemblyLoadContext loadContext)
        {
            this.assemblyNameToPathMap = assemblyNameToPathMap;
            this.loadContext = loadContext;
        }

        /// <summary>
        /// Creates a <see cref="StaticAssemblyResolver"/> from the specified assembly paths.
        /// </summary>
        /// <param name="assemblyPaths">The assembly paths.</param>
        /// <returns>A <see cref="StaticAssemblyResolver"/>.</returns>
        public static StaticAssemblyResolver Create(IEnumerable<string> assemblyPaths)
        {
            return Create(assemblyPaths, AssemblyLoadContext.Default);
        }

        /// <summary>
        /// Creates a <see cref="StaticAssemblyResolver" /> from the specified assembly paths.
        /// </summary>
        /// <param name="assemblyPaths">The assembly paths.</param>
        /// <param name="loadContext">The assembly load context.</param>
        /// <returns>A <see cref="StaticAssemblyResolver" />.</returns>
        public static StaticAssemblyResolver Create(IEnumerable<string> assemblyPaths, AssemblyLoadContext loadContext)
        {
            var assemblyNameToPathMap = assemblyPaths.ToDictionary(Path.GetFileNameWithoutExtension);
            return new StaticAssemblyResolver(assemblyNameToPathMap, loadContext);
        }

        /// <summary>
        /// Resolves the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName" /> to resolve.</param>
        /// <returns>An <see cref="Assembly" />.</returns>
        /// <exception cref="FileNotFoundException">Could not resolve an Assembly for the {assemblyName.Name}</exception>
        public Assembly Resolve(AssemblyName assemblyName)
        {
            if (this.TryResolve(assemblyName, out Assembly resolvedAssembly))
            {
                return resolvedAssembly;
            }

            throw new FileNotFoundException($"Could not resolve an Assembly for the {assemblyName.Name} assembly.");
        }

        /// <summary>
        /// Tries to resolve the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName" /> to resolve.</param>
        /// <param name="resolvedAssembly">The resolved <see cref="Assembly" />.</param>
        /// <returns><c>true</c> if resolution was successful, <c>false</c> otherwise.</returns>
        public bool TryResolve(AssemblyName assemblyName, out Assembly resolvedAssembly)
        {
            resolvedAssembly = null;

            if (this.assemblyNameToPathMap.TryGetValue(assemblyName.Name, out string resolvedAssemblyPath))
            {
                resolvedAssembly = this.loadContext.LoadFromAssemblyPath(resolvedAssemblyPath);
                return true;
            }

            return false;
        }
    }
}