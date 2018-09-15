using SymbioticTS.Abstractions;

namespace DiscoveryReferenceProject
{
    [TsInterface]
    public interface InterfaceWithUnannotatedInterface : InterfaceWithUnannotatedInterfaceInterface
    {
    }

    public interface InterfaceWithUnannotatedInterfaceInterface
    {
    }
}