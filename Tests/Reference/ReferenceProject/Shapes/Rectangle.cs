using SymbioticTS.Abstractions;

namespace ReferenceProject.Shapes
{
    [TsClass]
    public class Rectangle : IShape, IQuadrilateral
    {
        public Border Border { get; set; }

        public Color Color { get; }

        public int Sides { get; } = 4;

        public double Height { get; }

        public double Width { get; }

        public Rectangle(double height, double width, Color color)
        {
            this.Height = height;
            this.Width = width;
            this.Color = color;
        }

        public void WithBorder(Border border)
        {
            this.Border = border;
        }
    }
}