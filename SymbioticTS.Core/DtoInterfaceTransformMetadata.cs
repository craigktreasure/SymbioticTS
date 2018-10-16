namespace SymbioticTS.Core
{
    internal class DtoInterfaceTransformMetadata
    {
        /// <summary>
        /// Gets or sets the class symbol that the DTO interface represents.
        /// </summary>
        public TsTypeSymbol ClassSymbol { get; set; }

        /// <summary>
        /// Gets or sets the transform method accessor.
        /// </summary>
        public string TransformMethodAccessor { get; set; }

        /// <summary>
        /// Gets or sets the name of the transform method.
        /// </summary>
        public string TransformMethodName { get; set; }
    }
}