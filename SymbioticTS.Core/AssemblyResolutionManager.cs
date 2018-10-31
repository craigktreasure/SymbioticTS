using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace SymbioticTS.Core
{
    internal class AssemblyResolutionManager : IDisposable, IAssemblyResolver
    {
        private AssemblyLoadContext assemblyLoadContext;

        private List<IAssemblyResolver> assemblyResolvers = new List<IAssemblyResolver>();

        private bool disposedValue = false;

        private bool handlersRegistered = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResolutionManager"/> class.
        /// </summary>
        /// <param name="assemblyLoadContext">The assembly load context.</param>
        public AssemblyResolutionManager(AssemblyLoadContext assemblyLoadContext)
        {
            this.assemblyLoadContext = assemblyLoadContext;
        }

        /// <summary>
        /// Creates an <see cref="AssemblyResolutionManager"/> and registers the handlers for
        /// the specified <see cref="AssemblyLoadContext"/>.
        /// </summary>
        /// <returns>An <see cref="AssemblyResolutionManager"/>.</returns>
        public static AssemblyResolutionManager Create(AssemblyLoadContext assembloyLoadContext)
        {
            AssemblyResolutionManager assemblyResolver = new AssemblyResolutionManager(assembloyLoadContext);

            assemblyResolver.RegisterHandlers();

            return assemblyResolver;
        }

        /// <summary>
        /// Creates an <see cref="AssemblyResolutionManager"/> and registers the handlers for
        /// the default <see cref="AssemblyLoadContext"/>.
        /// </summary>
        /// <returns>An <see cref="AssemblyResolutionManager"/>.</returns>
        public static AssemblyResolutionManager CreateDefault()
        {
            return Create(AssemblyLoadContext.Default);
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Registers this assembly resolver handlers.
        /// </summary>
        public void RegisterHandlers()
        {
            if (!this.handlersRegistered)
            {
                this.handlersRegistered = true;

                this.assemblyLoadContext.Resolving += this.AssemblyResolvingHandler;
            }
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
            foreach (IAssemblyResolver assemblyResolver in this.assemblyResolvers)
            {
                if (assemblyResolver.TryResolve(assemblyName, out resolvedAssembly))
                {
                    return true;
                }
            }

            resolvedAssembly = null;
            return false;
        }

        /// <summary>
        /// Unregisters the assembly resolver handlers.
        /// </summary>
        public void UnregisterHandlers()
        {
            if (this.handlersRegistered)
            {
                this.handlersRegistered = false;

                this.assemblyLoadContext.Resolving -= this.AssemblyResolvingHandler;
            }
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
                    this.UnregisterHandlers();
                }

                this.assemblyResolvers = null;
                this.assemblyLoadContext = null;

                this.disposedValue = true;
            }
        }

        private Assembly AssemblyResolvingHandler(AssemblyLoadContext assemblyLoadContext, AssemblyName resolvingAssemblyName)
        {
            foreach (IAssemblyResolver assemblyResolver in this.assemblyResolvers)
            {
                if (assemblyResolver.TryResolve(resolvingAssemblyName, out Assembly resolvedAssembly))
                {
                    return resolvedAssembly;
                }
            }

            return null;
        }
    }
}