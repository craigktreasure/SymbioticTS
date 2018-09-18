using SymbioticTS.Abstractions;

namespace ReferenceProject.Shapes
{
    [TsInterface]
    public interface IShape
    {
        [TsProperty(IsOptional = TsPropertyValueOptions.Yes)]
        Border Border { get; }

        [TsProperty(IsReadOnly = TsPropertyValueOptions.Yes)]
        Color Color { get; }

        int Sides { get; }
    }
}