using SymbioticTS.Abstractions;

namespace UnitTestReferenceLibrary.WithOverlap
{
    [TsDto]
    public class Circle : ICircle
    {
        public double Diameter { get; }
    }

    public interface ICircle
    {
        double Diameter { get; }
    }
}