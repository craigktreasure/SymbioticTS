using SymbioticTS.Abstractions;
using System;

namespace UnitTestReferenceLibrary
{
    [TsDto]
    public class DtoWithDateTimeTestClass
    {
        public int Count { get; set; }

        public DateTime Birthdate { get; set; }
    }
}