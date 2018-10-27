using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal sealed class StaticAssemblyResolver : IAssemblyResolver
    {
        private readonly IReadOnlyDictionary<AssemblyName, string> assemblyNameToPathMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticAssemblyResolver"/> class.
        /// </summary>
        /// <param name="assemblyNameToPathMap">The assembly name to path map.</param>
        private StaticAssemblyResolver(IReadOnlyDictionary<AssemblyName, string> assemblyNameToPathMap)
        {
            this.assemblyNameToPathMap = assemblyNameToPathMap;
        }

        /// <summary>
        /// Creates a <see cref="StaticAssemblyResolver"/> from the specified assembly paths.
        /// </summary>
        /// <param name="assemblyPaths">The assembly paths.</param>
        /// <returns>A <see cref="StaticAssemblyResolver"/>.</returns>
        public static StaticAssemblyResolver Create(IEnumerable<string> assemblyPaths)
        {
            return new StaticAssemblyResolver(assemblyPaths.ToDictionary(AssemblyName.GetAssemblyName, AssemblyNameComparer.Instance));
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

            if (this.assemblyNameToPathMap.TryGetValue(assemblyName, out string resolvedAssemblyPath))
            {
                resolvedAssembly = Assembly.LoadFrom(resolvedAssemblyPath);
                return true;
            }

            return false;
        }
    }
}