using SymbioticTS.Core.IOAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SymbioticTS.TestLibrary
{
    internal class AssemblyResourceFileSource : IFileSource
    {
        /// <summary>
        /// The assembly resource reader.
        /// </summary>
        private readonly AssemblyResourceReader assemblyResourceReader;

        /// <summary>
        /// The resources.
        /// </summary>
        private readonly HashSet<string> resources;

        /// <summary>
        /// Gets the files.
        /// </summary>
        public IEnumerable<string> Files => this.resources;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourceFileSource"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AssemblyResourceFileSource(Assembly assembly)
            : this(new AssemblyResourceReader(assembly))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourceFileSource"/> class.
        /// </summary>
        /// <param name="assemblyResourceReader">The assembly resource reader.</param>
        public AssemblyResourceFileSource(AssemblyResourceReader assemblyResourceReader)
            : this(assemblyResourceReader, assemblyResourceReader.GetResources())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourceFileSource"/> class.
        /// </summary>
        /// <param name="assemblyResourceReader">The assembly resource reader.</param>
        /// <param name="resourceNames">The resource names.</param>
        public AssemblyResourceFileSource(AssemblyResourceReader assemblyResourceReader, IEnumerable<string> resourceNames)
        {
            this.assemblyResourceReader = assemblyResourceReader;
            this.resources = new HashSet<string>(resourceNames, StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets an <see cref="AssemblyResourceFileSource"/> from the specified <see cref="Assembly"/> only
        /// including resources with the specified resource qualifier.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="resourceQualifier">The resource qualifier.</param>
        /// <returns>AssemblyResourceFileSource.</returns>
        public static AssemblyResourceFileSource WithResourceQualifier(Assembly assembly, string resourceQualifier)
        {
            AssemblyResourceReader assemblyResourceReader = new AssemblyResourceReader(assembly);

            IEnumerable<string> files = assemblyResourceReader.GetResources()
                .Where(n => n.StartsWith(resourceQualifier));

            return new AssemblyResourceFileSource(assemblyResourceReader, files);
        }

        /// <summary>
        /// Gets an <see cref="AssemblyResourceFileSource"/> from the specified <see cref="Assembly"/> contianing
        /// the specified resources.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="resources">The resources.</param>
        /// <returns>AssemblyResourceFileSource.</returns>
        public static AssemblyResourceFileSource WithResources(Assembly assembly, IEnumerable<string> resources)
        {
            AssemblyResourceReader assemblyResourceReader = new AssemblyResourceReader(assembly);

            return new AssemblyResourceFileSource(assemblyResourceReader, resources);
        }

        /// <summary>
        /// Gets the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file <see cref="T:System.IO.Stream" />.</returns>
        /// <exception cref="FileNotFoundException">The file could not be found.</exception>
        public Stream OpenFile(string fileName)
        {
            if (!this.resources.Contains(fileName))
            {
                throw new FileNotFoundException("The file could not be found.", fileName);
            }

            return this.assemblyResourceReader.GetResource(fileName);
        }
    }
}