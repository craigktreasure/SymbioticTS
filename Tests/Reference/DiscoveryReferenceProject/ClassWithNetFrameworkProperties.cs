using SymbioticTS.Abstractions;
using System;
using System.Collections.Generic;

namespace DiscoveryReferenceProject
{
    [TsClass]
    public class ClassWithNetFrameworkProperties
    {
        public DateTime DateTimeProperty { get; set; }

        public IEnumerable<int> EnumerableIntProperty { get; set; }

        public int[] IntArrayProperty { get; set; }

        public int IntProperty { get; set; }

        public int? NullableIntProperty { get; set; }

        public string StringProperty { get; set; }
    }
}