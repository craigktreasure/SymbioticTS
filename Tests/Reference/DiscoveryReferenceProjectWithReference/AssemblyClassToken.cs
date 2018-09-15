using SymbioticTS.Abstractions;

namespace DiscoveryReferenceProjectWithReference
{
    internal class AssemblyClassToken
    {
        [TsProperty] // Force reference to SymbioticTS.Abstractions
        internal DiscoveryReferenceProject.AssemblyClassToken DependencyToken { get; } // Force reference to DiscoveryReferenceProject
    }
}