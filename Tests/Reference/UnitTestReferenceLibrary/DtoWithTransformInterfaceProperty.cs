using SymbioticTS.Abstractions;

namespace UnitTestReferenceLibrary
{
    [TsDto]
    public class DtoWithTransformInterfaceProperty
    {
        public IRequireTransform Property { get; set; }
    }
}