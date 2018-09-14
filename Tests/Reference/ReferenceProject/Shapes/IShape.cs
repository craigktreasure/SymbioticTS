using SymbioticTS.Abstractions;

namespace ReferenceProject.Shapes
{
    [TsInterface]
    public interface IShape
    {
        Border Border { get; }

        Color Color { get; }

        int Sides { get; }
    }
}