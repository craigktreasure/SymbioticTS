using SymbioticTS.Abstractions;
using System;
using System.Collections.Generic;

namespace UnitTestReferenceLibrary
{
    [TsDto]
    public class DtoWithDateTimeCollectionTestClass
    {
        public int Count { get; set; }

        public IEnumerable<DateTime> Birthdate { get; set; }
    }
}