using SymbioticTS.Abstractions;

namespace ScenarioReferenceProject.Shapes
{
    [TsEnum(name: "ShapeBorder", asConstant: true)]
    public enum Border
    {
        None,
        Solid,
        Dotted,
    }
}