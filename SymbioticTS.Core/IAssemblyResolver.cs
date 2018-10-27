using System.Reflection;

namespace SymbioticTS.Core
{
    internal interface IAssemblyResolver
    {
        /// <summary>
        /// Resolves the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to resolve.</param>
        /// <returns>An <see cref="Assembly"/>.</returns>
        Assembly Resolve(AssemblyName assemblyName);

        /// <summary>
        /// Tries to resolve the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to resolve.</param>
        /// <param name="resolvedAssembly">The resolved <see cref="Assembly"/>.</param>
        /// <returns><c>true</c> if resolution was successful, <c>false</c> otherwise.</returns>
        bool TryResolve(AssemblyName assemblyName, out Assembly resolvedAssembly);
    }
}