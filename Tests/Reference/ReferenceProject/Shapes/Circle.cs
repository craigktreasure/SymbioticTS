using SymbioticTS.Abstractions;

namespace ReferenceProject.Shapes
{
    [TsClass]
    public class Circle : IShape
    {
        public Border Border { get; set; }

        public Color Color { get; }

        public int Sides { get; } = 0;

        public double Diameter { get; }

        public Circle(double diameter, Color color)
        {
            this.Diameter = diameter;
            this.Color = color;
        }

        public void WithBorder(Border border)
        {
            this.Border = border;
        }
    }
}