using SymbioticTS.Abstractions;

namespace DiscoveryReferenceProject
{
    [TsClass]
    public class ClassWithUnannotatedProperties
    {
        public enum UnannotatedEnum { }

        public interface IUnannotatedInterface { }

        public class UnannotatedClass { }

        public UnannotatedClass ClassProperty { get; set; }

        public UnannotatedEnum EnumProperty { get; set; }

        public IUnannotatedInterface InterfaceProperty { get; set; }
    }
}