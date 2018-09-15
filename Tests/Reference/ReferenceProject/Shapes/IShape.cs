using SymbioticTS.Abstractions;

namespace ReferenceProject.Shapes
{
    [TsInterface]
    public interface IShape
    {
        [TsProperty(PropertyOptions.Optional)]
        Border Border { get; }

        [TsProperty(PropertyOptions.ReadOnly)]
        Color Color { get; }

        int Sides { get; }
    }
}