using SymbioticTS.Abstractions;

namespace UnitTestReferenceLibrary
{
    [TsDto]
    public class DtoWithReadOnlyPropertyTestClass
    {
        [TsProperty(IsReadOnly = TsPropertyValueOptions.Yes)]
        public int Count { get; }

        public string Name { get; set; }
    }
}