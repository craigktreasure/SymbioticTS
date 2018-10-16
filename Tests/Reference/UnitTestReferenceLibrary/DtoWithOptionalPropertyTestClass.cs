using SymbioticTS.Abstractions;

namespace UnitTestReferenceLibrary
{
    [TsDto]
    public class DtoWithOptionalPropertyTestClass
    {
        [TsProperty(IsOptional = TsPropertyValueOptions.Yes)]
        public int Count { get; }

        public string Name { get; set; }
    }
}