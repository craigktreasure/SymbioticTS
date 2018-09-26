using SymbioticTS.Abstractions;

namespace DiscoveryReferenceProject
{
    [TsClass]
    public class ClassWithUnannotatedNestedProperties
    {
        public enum UnannotatedPropertyEnum { }

        public interface IUnannotatedPropertyInterface { }

        public class UnannotatedClass
        {
            public UnannotatedPropertyEnum EnumProperty { get; set; }

            public IUnannotatedPropertyInterface InterfaceProperty { get; set; }

            public UnannotatedPropertyClass ClassProperty { get; set; }
        }

        public class UnannotatedPropertyClass { }

        public UnannotatedClass ClassProperty { get; set; }
    }
}