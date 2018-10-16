using SymbioticTS.Abstractions;

namespace UnitTestReferenceLibrary
{
    [TsDto]
    public class DtoWithTransformInterfaceCollectionProperty
    {
        public IRequireTransform[] Property { get; set; }
    }
}