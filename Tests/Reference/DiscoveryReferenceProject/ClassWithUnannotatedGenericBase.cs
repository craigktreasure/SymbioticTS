using SymbioticTS.Abstractions;

namespace DiscoveryReferenceProject
{
    // Generics are not currently supported
    // [TsClass]
    public class ClassWithUnannotatedGenericBase : ClassWithUnannotatedGenericBaseBase<ClassWithUnannotatedGenericBaseBaseGeneric>
    {
    }

    public class ClassWithUnannotatedGenericBaseBase<T> { }

    public class ClassWithUnannotatedGenericBaseBaseGeneric { }
}