using SymbioticTS.Abstractions;

namespace DiscoveryReferenceProject
{
    [TsClass]
    public class ClassWithUnannotatedArrayClassProperty
    {
        public class UnnanotatedArrayClass { }

        public UnnanotatedArrayClass[] ArrayClassProperty { get; set; }
    }
}