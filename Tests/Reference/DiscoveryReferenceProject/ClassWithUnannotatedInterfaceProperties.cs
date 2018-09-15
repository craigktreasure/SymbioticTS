using SymbioticTS.Abstractions;

namespace DiscoveryReferenceProject
{
    public interface ClassWithUnannotatedInterfacePropertiesInterface
    {
        ClassWithUnannotatedInterfaceProperties.UnannotatedClass ClassProperty { get; }
        ClassWithUnannotatedInterfaceProperties.UnannotatedEnum EnumProperty { get; }
        ClassWithUnannotatedInterfaceProperties.IUnannotatedInterface InterfaceProperty { get; }
    }

    [TsClass]
    public class ClassWithUnannotatedInterfaceProperties : ClassWithUnannotatedInterfacePropertiesInterface
    {
        public enum UnannotatedEnum { }

        public interface IUnannotatedInterface { }

        public class UnannotatedClass { }

        public UnannotatedClass ClassProperty { get; }

        public UnannotatedEnum EnumProperty { get; }

        public IUnannotatedInterface InterfaceProperty { get; }
    }
}