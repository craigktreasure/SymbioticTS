using System;
using System.Collections.Generic;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class CompositeAssemblyResolver : IAssemblyResolver
    {
        private List<IAssemblyResolver> assemblyResolvers = new List<IAssemblyResolver>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeAssemblyResolver"/> class.
        /// </summary>
        public CompositeAssemblyResolver()
        {
        }

        /// <summary>
        /// Adds the assembly resolver.
        /// </summary>
        /// <param name="assemblyResolver">The assembly resolver.</param>
        public void AddAssemblyResolver(IAssemblyResolver assemblyResolver)
        {
            this.assemblyResolvers.Add(assemblyResolver);
        }

        /// <summary>
        /// Resolves the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName" /> to resolve.</param>
        /// <returns>An <see cref="Assembly" />.</returns>
        /// <exception cref="DllNotFoundException">Could not resolve an Assembly for the {assemblyName.Name}</exception>
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
            foreach (IAssemblyResolver resolver in this.assemblyResolvers)
            {
                if (resolver.TryResolve(assemblyName, out resolvedAssembly))
                {
                    return true;
                }
            }

            resolvedAssembly = null;
            return false;
        }

        /// <summary>
        /// Tries to resolve the specified assembly path.
        /// </summary>
        /// <param name="assemblyPath">The assembly path to resolve.</param>
        /// <param name="resolvedAssembly">The resolved <see cref="Assembly" />.</param>
        /// <returns><c>true</c> if resolution was successful, <c>false</c> otherwise.</returns>
        public bool TryResolve(string assemblyPath, out Assembly resolvedAssembly)
        {
            foreach (IAssemblyResolver resolver in this.assemblyResolvers)
            {
                if (resolver.TryResolve(assemblyPath, out resolvedAssembly))
                {
                    return true;
                }
            }

            resolvedAssembly = null;
            return false;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                }

                if (this.assemblyResolvers != null)
                {
                    foreach (IAssemblyResolver assemblyResolver in this.assemblyResolvers)
                    {
                        if (assemblyResolver is IDisposable disposableAssemblyResolver)
                        {
                            disposableAssemblyResolver.Dispose();
                        }
                    }

                    this.assemblyResolvers = null;
                }

                this.disposedValue = true;
            }
        }

        #endregion IDisposable Support
    }
}