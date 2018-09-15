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
            public UnannotatedPropertyEnum Enum { get; set; }

            public IUnannotatedPropertyInterface Interface { get; set; }

            public UnannotatedPropertyClass Property { get; set; }
        }

        public class UnannotatedPropertyClass { }

        public UnannotatedClass Property { get; set; }
    }
}