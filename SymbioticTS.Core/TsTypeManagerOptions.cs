namespace SymbioticTS.Core
{
    internal class TsTypeManagerOptions : ISymbolLoadOptions
    {
        /// <summary>
        /// Gets the default options.
        /// </summary>
        /// <value>The default.</value>
        public static TsTypeManagerOptions Default => new TsTypeManagerOptions();

        /// <summary>
        /// Gets or sets the property name style.
        /// </summary>
        /// <value>The property name style.</value>
        public PropertyNameStyle PropertyNameStyle { get; set; } = PropertyNameStyle.CamelCase;
    }
}