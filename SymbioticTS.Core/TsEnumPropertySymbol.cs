namespace SymbioticTS.Core
{
    internal class TsEnumItemSymbol
    {
        /// <summary>
        /// Gets the enumeration item name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the enumeration item value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsEnumItemSymbol"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public TsEnumItemSymbol(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}