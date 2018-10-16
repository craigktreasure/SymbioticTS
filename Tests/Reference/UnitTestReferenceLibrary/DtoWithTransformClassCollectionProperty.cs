using SymbioticTS.Abstractions;
using System.Collections.Generic;

namespace UnitTestReferenceLibrary
{
    [TsDto]
    public class DtoWithTransformClassCollectionProperty
    {
        public IEnumerable<ClassRequiresTransform> Transform { get; set; }
    }
}