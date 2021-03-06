﻿using SymbioticTS.Abstractions;

namespace ScenarioReferenceProject.Shapes
{
    [TsClass]
    public class Circle : BaseShape
    {
        public double Diameter { get; }

        public Circle(double diameter, Color color)
            : base(0)
        {
            this.Diameter = diameter;
            this.Color = color;
        }
    }
}