using SymbioticTS.Abstractions;
using System;

namespace Example
{
    [TsDto]
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }
    }
}
