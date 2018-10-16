using SymbioticTS.Abstractions;

namespace ScenarioReferenceProject.Shapes
{
    [TsClass]
    public class Rectangle : BaseShape, IQuadrilateral
    {
        public double Height { get; }

        public double Width { get; }

        public Rectangle(double height, double width, Color color)
            : base(4)
        {
            this.Height = height;
            this.Width = width;
            this.Color = color;
        }
    }
}