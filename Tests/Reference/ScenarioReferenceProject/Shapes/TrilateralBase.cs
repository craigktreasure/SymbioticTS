namespace ScenarioReferenceProject.Shapes
{
    public abstract class TrilateralBase : BaseShape, ITrilateral
    {
        protected TrilateralBase()
            : base(3)
        {
        }
    }
}