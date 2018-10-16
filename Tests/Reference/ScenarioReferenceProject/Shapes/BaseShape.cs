namespace ScenarioReferenceProject.Shapes
{
    public abstract class BaseShape : IShape
    {
        public Border Border { get; set; }

        // Should be readonly
        public Color Color { get; protected set; }

        public int Sides { get; }

        protected BaseShape(int sides)
        {
            this.Sides = sides;
        }
    }
}