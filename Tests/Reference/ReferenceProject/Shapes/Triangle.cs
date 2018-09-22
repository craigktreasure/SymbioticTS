using SymbioticTS.Abstractions;

namespace ReferenceProject.Shapes
{
    [TsClass(flattenInheritance: true)]
    internal class Triangle : TrilateralBase, ITrilateral
    {
        public Triangle(Color color)
        {
            this.Color = color;
        }
    }
}