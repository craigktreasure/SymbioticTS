namespace SymbioticTS.Core
{
    internal enum PropertyNameStyle
    {
        /// <summary>
        /// Leave property names as-is.
        /// </summary>
        AsIs,

        /// <summary>
        /// Convert property names to camel case. Common for JSON.
        /// </summary>
        CamelCase,

        /// <summary>
        /// Convert property names to pascal case.
        /// </summary>
        PascalCase
    }
}