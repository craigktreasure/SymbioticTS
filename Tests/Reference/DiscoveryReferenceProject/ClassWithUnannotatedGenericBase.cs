using SymbioticTS.Abstractions;

namespace DiscoveryReferenceProject
{
    [TsClass]
    public class ClassWithUnannotatedGenericBase : ClassWithUnannotatedGenericBaseBase<ClassWithUnannotatedGenericBaseBaseGeneric>
    {
    }

    public class ClassWithUnannotatedGenericBaseBase<T> { }

    public class ClassWithUnannotatedGenericBaseBaseGeneric { }
}