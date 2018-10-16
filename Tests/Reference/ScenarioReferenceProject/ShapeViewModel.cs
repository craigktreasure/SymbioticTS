using ScenarioReferenceProject.Shapes;
using SymbioticTS.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace ScenarioReferenceProject
{
    [TsDto]
    public class ShapeViewModel : BaseViewModel
    {
        public IShape[] Shapes { get; set; }

        public int TotalShapes => this.Shapes?.Length ?? 0;

        public IEnumerable<Rectangle> Rectangles => this.Shapes?
            .Where(s => s is Rectangle)
            .Cast<Rectangle>();

        public int? TotalRectangles => this.Rectangles?.Count();

        public IReadOnlyList<Circle> Circles => this.Shapes?
            .Where(s => s is Circle)
            .Cast<Circle>()
            .ToList();

        public long TotalCircles => this.Circles?.Count ?? 0;
    }
}