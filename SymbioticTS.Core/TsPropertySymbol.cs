using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SymbioticTS.Core
{
    [DebuggerDisplay("{Name}")]
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
        public string Name { get; }

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
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="propertyMetadata">The property metadata.</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal TsPropertySymbol(string name, TsTypeSymbol type, TsPropertyMetadata propertyMetadata)
        {
            this.Name = name;
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.PropertyMetadata = propertyMetadata ?? throw new ArgumentNullException(nameof(propertyMetadata));
        }

        /// <summary>
        /// Loads a <see cref="TsPropertySymbol" /> from the specified <see cref="PropertyInfo" /> and <see cref="TsSymbolLookup" />.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="TsPropertySymbol" />.</returns>
        /// <exception cref="NotSupportedException">The type {property.PropertyType.FullName} of {property.Name}</exception>
        internal static TsPropertySymbol LoadFrom(PropertyInfo property, TsSymbolLookup symbolLookup, ISymbolLoadOptions options)
        {
            TsPropertyMetadata propertyMetadata = TsPropertyMetadata.LoadFrom(property);

            return LoadFrom(propertyMetadata, symbolLookup, options);
        }

        /// <summary>
        /// Loads a <see cref="TsPropertySymbol" /> from the specified <see cref="TsPropertyMetadata" /> and <see cref="TsSymbolLookup" />.
        /// </summary>
        /// <param name="propertyMetadata">The property metadata.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="TsPropertySymbol" />.</returns>
        /// <exception cref="NotSupportedException">The type {property.PropertyType.FullName} of {property.Name}</exception>
        internal static TsPropertySymbol LoadFrom(TsPropertyMetadata propertyMetadata, TsSymbolLookup symbolLookup, ISymbolLoadOptions options)
        {
            if (!symbolLookup.TryResolveSymbol(propertyMetadata.Property.PropertyType, out TsTypeSymbol type))
            {
                throw new NotSupportedException($"The type {propertyMetadata.Property.PropertyType.FullName} of {propertyMetadata.Name} is not supported.");
            }

            string name = GetPropertyName(propertyMetadata.Property, options);

            return new TsPropertySymbol(name, type, propertyMetadata);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="NotSupportedException">Unsupported name style: {options.NameStyle}</exception>
        private static string GetPropertyName(PropertyInfo property, ISymbolLoadOptions options)
        {
            string result = null;

            switch (options.PropertyNameStyle)
            {
                case PropertyNameStyle.AsIs:
                    result = property.Name;
                    break;

                case PropertyNameStyle.CamelCase:
                    result = property.Name.Camelize();
                    break;

                case PropertyNameStyle.PascalCase:
                    result = property.Name.Pascalize();
                    break;

                default:
                    throw new NotSupportedException($"Unsupported name style: {options.PropertyNameStyle}.");
            }

            if (TypeScriptLanguage.IsReservedKeyword(result))
            {
                throw new ArgumentException($"The {property.DeclaringType.FullName}.{property.Name} property results in an invalid field name with the {options.PropertyNameStyle} name style: {result}.");
            }

            return result;
        }
    }

    internal class TsPropertySymbolComparer : IEqualityComparer<TsPropertySymbol>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(TsPropertySymbol x, TsPropertySymbol y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.PropertyMetadata.Property == y.PropertyMetadata.Property;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(TsPropertySymbol obj)
        {
            return obj.PropertyMetadata.Property.GetHashCode();
        }
    }
}