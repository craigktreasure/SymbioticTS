using System;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class TsPropertySymbol
    {
        /// <summary>
        /// Gets a value indicating whether this instance is optional.
        /// </summary>
        /// <value><c>true</c> if this instance is optional; otherwise, <c>false</c>.</value>
        public bool IsOptional => this.PropertyMetadata.IsOptional;

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => this.PropertyMetadata.IsReadOnly;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => this.PropertyMetadata.Name;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public TsTypeSymbol Type { get; }

        /// <summary>
        /// Gets the property metadata.
        /// </summary>
        /// <value>The property metadata.</value>
        internal TsPropertyMetadata PropertyMetadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsPropertySymbol" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyMetadata">The property metadata.</param>
        internal TsPropertySymbol(TsTypeSymbol type, TsPropertyMetadata propertyMetadata)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.PropertyMetadata = propertyMetadata ?? throw new ArgumentNullException(nameof(propertyMetadata));
        }

        /// <summary>
        /// Loads a <see cref="TsPropertySymbol"/> from the specified <see cref="PropertyInfo"/> and <see cref="TsSymbolLookup"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <returns>A <see cref="TsPropertySymbol"/>.</returns>
        /// <exception cref="NotSupportedException">The type {property.PropertyType.FullName} of {property.Name}</exception>
        internal static TsPropertySymbol LoadFrom(PropertyInfo property, TsSymbolLookup symbolLookup)
        {
            TsPropertyMetadata propertyMetadata = TsPropertyMetadata.LoadFrom(property);

            return LoadFrom(propertyMetadata, symbolLookup);
        }

        /// <summary>
        /// Loads a <see cref="TsPropertySymbol" /> from the specified <see cref="TsPropertyMetadata" /> and <see cref="TsSymbolLookup" />.
        /// </summary>
        /// <param name="propertyMetadata">The property metadata.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <returns>A <see cref="TsPropertySymbol" />.</returns>
        /// <exception cref="NotSupportedException">The type {property.PropertyType.FullName} of {property.Name}</exception>
        internal static TsPropertySymbol LoadFrom(TsPropertyMetadata propertyMetadata, TsSymbolLookup symbolLookup)
        {
            if (!symbolLookup.TryResolveSymbol(propertyMetadata.Property.PropertyType, out TsTypeSymbol type))
            {
                throw new NotSupportedException($"The type {propertyMetadata.Property.PropertyType.FullName} of {propertyMetadata.Name} is not supported.");
            }

            return new TsPropertySymbol(type, propertyMetadata);
        }
    }
}