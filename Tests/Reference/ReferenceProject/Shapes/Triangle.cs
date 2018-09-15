using SymbioticTS.Abstractions;

namespace ReferenceProject.Shapes
{
    [TsClass(flattenInheritance: true)]
    internal class Triangle : BaseShape
    {
        public Triangle(Color color)
            : base(3)
        {
            this.Color = color;
        }
    }
}