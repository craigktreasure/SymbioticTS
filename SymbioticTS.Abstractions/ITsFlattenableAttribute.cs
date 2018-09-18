namespace SymbioticTS.Abstractions
{
    internal interface ITsFlattenableAttribute
    {
        /// <summary>
        /// Gets a value indicating that the generated object will
        /// include all properties, but will not inherit from any objects.
        /// </summary>
        /// <value><c>true</c> for flattened inheritance; otherwise, <c>false</c>.</value>
        bool FlattenInheritance { get; }
    }
}