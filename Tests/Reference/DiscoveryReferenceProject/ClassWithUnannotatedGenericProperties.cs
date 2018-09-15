using SymbioticTS.Abstractions;
using System.Collections.Generic;

namespace DiscoveryReferenceProject
{
    [TsClass]
    public class ClassWithUnannotatedGenericProperties
    {
        public struct UnnanotatedNullableGenericStruct { }

        public class UnnanotatedGenericClass { }

        public IEnumerable<UnnanotatedGenericClass> GenericClassProperty { get; set; }

        public UnnanotatedNullableGenericStruct? NullableStructProperty { get; set; }
    }
}