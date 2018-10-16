using SymbioticTS.Abstractions;

namespace UnitTestReferenceLibrary
{
    [TsDto]
    public class DtoWithTransformClassProperty
    {
        public ClassRequiresTransform Transform { get; set; }
    }
}