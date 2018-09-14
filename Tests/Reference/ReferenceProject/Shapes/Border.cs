using SymbioticTS.Abstractions;

namespace ReferenceProject.Shapes
{
    [TsEnum(name: "ShapeBorder", asConstant: true)]
    public enum Border
    {
        None,
        Solid,
        Dotted,
    }
}