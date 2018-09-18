namespace SymbioticTS.Abstractions
{
    internal interface ITsNameableAttribute
    {
        /// <summary>
        /// Gets the name of the TypeScript object generated.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }
}