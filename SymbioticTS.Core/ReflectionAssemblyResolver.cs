using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal sealed class ReflectionAssemblyResolver : IAssemblyResolver
    {
        private readonly MetadataLoadContext metadataLoadContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionAssemblyResolver" /> class.
        /// </summary>
        /// <param name="assemblyPaths">The assembly paths.</param>
        private ReflectionAssemblyResolver(IEnumerable<string> assemblyPaths)
        {
            string coreAssemblyLocation = typeof(object).Assembly.Location;

            List<string> assemblies = new List<string>(assemblyPaths);
            assemblies.Add(coreAssemblyLocation);

            MetadataAssemblyResolver assemblyResolver = new PathAssemblyResolver(assemblies);

            this.metadataLoadContext = new MetadataLoadContext(assemblyResolver);
        }

        /// <summary>
        /// Creates a <see cref="ReflectionAssemblyResolver"/> from the specified assembly paths.
        /// </summary>
        /// <param name="assemblyPaths">The assembly paths.</param>
        /// <returns>A <see cref="ReflectionAssemblyResolver"/>.</returns>
        public static ReflectionAssemblyResolver Create(IEnumerable<string> assemblyPaths)
        {
            return new ReflectionAssemblyResolver(assemblyPaths);
        }

        /// <summary>
        /// Creates a <see cref="ReflectionAssemblyResolver"/> from the specified directory path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><see cref="ReflectionAssemblyResolver"/>.</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static ReflectionAssemblyResolver CreateFromDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(path);
            }

            IEnumerable<string> assemblyPaths = Directory.EnumerateFiles(path, "*.dll", SearchOption.TopDirectoryOnly);

            return Create(assemblyPaths);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.metadataLoadContext.Dispose();
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

            throw new DllNotFoundException($"Could not resolve an Assembly for the {assemblyName.Name} assembly.");
        }

        /// <summary>
        /// Resolves the specified assembly path.
        /// </summary>
        /// <param name="assemblyPath">The assembly path to resolve.</param>
        /// <returns>An <see cref="Assembly" />.</returns>
        /// <exception cref="DllNotFoundException">Could not resolve an Assembly at {assemblyPath}</exception>
        public Assembly Resolve(string assemblyPath)
        {
            if (this.TryResolve(assemblyPath, out Assembly resolvedAssembly))
            {
                return resolvedAssembly;
            }

            throw new DllNotFoundException($"Could not resolve an Assembly at {assemblyPath}.");
        }

        /// <summary>
        /// Tries to resolve the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName" /> to resolve.</param>
        /// <param name="resolvedAssembly">The resolved <see cref="Assembly" />.</param>
        /// <returns><c>true</c> if resolution was successful, <c>false</c> otherwise.</returns>
        public bool TryResolve(AssemblyName assemblyName, out Assembly resolvedAssembly)
        {
            resolvedAssembly = this.metadataLoadContext.LoadFromAssemblyName(assemblyName);

            return resolvedAssembly != null;
        }

        /// <summary>
        /// Tries to resolve the specified assembly path.
        /// </summary>
        /// <param name="assemblyPath">The assembly path to resolve.</param>
        /// <param name="resolvedAssembly">The resolved <see cref="Assembly" />.</param>
        /// <returns><c>true</c> if resolution was successful, <c>false</c> otherwise.</returns>
        public bool TryResolve(string assemblyPath, out Assembly resolvedAssembly)
        {
            resolvedAssembly = this.metadataLoadContext.LoadFromAssemblyPath(assemblyPath);

            return resolvedAssembly != null;
        }
    }
}