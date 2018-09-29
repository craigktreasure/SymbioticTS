using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SymbioticTS.TestLibrary
{
    internal class AssemblyResourceReader
    {
        /// <summary>
        /// The assembly.
        /// </summary>
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourceReader"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AssemblyResourceReader(Assembly assembly)
        {
            this.assembly = assembly;
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns><see cref="string"/>.</returns>
        public string GetResource(string resourceName)
        {
            using (Stream resourceStream = this.assembly.GetManifestResourceStream(resourceName))
            using (StreamReader streamReader = new StreamReader(resourceStream, true))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <returns><see cref="IReadOnlyList{T}"/> of <see cref="string"/>.</returns>
        public IReadOnlyList<string> GetResources()
        {
            return this.assembly.GetManifestResourceNames();
        }
    }
}