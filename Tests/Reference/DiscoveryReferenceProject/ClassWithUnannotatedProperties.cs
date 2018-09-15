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

        public UnannotatedEnum Enum { get; set; }

        public IUnannotatedInterface Property { get; set; }
    }
}