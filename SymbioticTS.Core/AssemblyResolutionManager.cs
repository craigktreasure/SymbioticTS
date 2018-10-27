using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace SymbioticTS.Core
{
    internal class AssemblyResolutionManager : IDisposable
    {
        private readonly AssemblyLoadContext assemblyLoadContext;

        private List<IAssemblyResolver> assemblyResolvers = new List<IAssemblyResolver>();

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

        #region IDisposable Support

        private bool disposedValue = false;

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.UnregisterHandlers();
                }

                this.assemblyResolvers = null;

                this.disposedValue = true;
            }
        }

        #endregion IDisposable Support
    }
}