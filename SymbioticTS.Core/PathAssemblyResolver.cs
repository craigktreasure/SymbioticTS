using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class PathAssemblyResolver : IAssemblyResolver
    {
        private readonly StaticAssemblyResolver assemblyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathAssemblyResolver" /> class.
        /// </summary>
        /// <param name="assemblyResolver">The assembly resolver.</param>
        /// <exception cref="ArgumentNullException">assemblyResolver</exception>
        private PathAssemblyResolver(StaticAssemblyResolver assemblyResolver)
        {
            this.assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
        }

        /// <summary>
        /// Creates a <see cref="PathAssemblyResolver"/> from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>PathAssemblyResolver.</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static PathAssemblyResolver Create(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(path);
            }

            IEnumerable<string> assemblyPaths = Directory.EnumerateFiles(path, "*.dll", SearchOption.TopDirectoryOnly);

            return new PathAssemblyResolver(StaticAssemblyResolver.Create(assemblyPaths));
        }

        /// <summary>
        /// Resolves the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName" /> to resolve.</param>
        /// <returns>An <see cref="Assembly" />.</returns>
        public Assembly Resolve(AssemblyName assemblyName)
        {
            return this.assemblyResolver.Resolve(assemblyName);
        }

        /// <summary>
        /// Tries to resolve the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName" /> to resolve.</param>
        /// <param name="resolvedAssembly">The resolved <see cref="Assembly" />.</param>
        /// <returns><c>true</c> if resolution was successful, <c>false</c> otherwise.</returns>
        public bool TryResolve(AssemblyName assemblyName, out Assembly resolvedAssembly)
        {
            return this.assemblyResolver.TryResolve(assemblyName, out resolvedAssembly);
        }
    }
}