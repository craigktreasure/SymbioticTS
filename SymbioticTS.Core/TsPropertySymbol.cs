using SymbioticTS.Abstractions;
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
        public bool IsOptional { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly { get; }

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
        /// Initializes a new instance of the <see cref="TsPropertySymbol"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="isOptional">if set to <c>true</c>, the instance is optional.</param>
        /// <param name="isReadOnly">if set to <c>true</c>, the instance is read only.</param>
        public TsPropertySymbol(string name, TsTypeSymbol type, bool isOptional, bool isReadOnly)
        {
            this.Name = name;
            this.Type = type;
            this.IsOptional = isOptional;
            this.IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// Loads a <see cref="TsPropertySymbol"/> from the specified <see cref="PropertyInfo"/> and <see cref="TsSymbolLookup"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <returns>A <see cref="TsPropertySymbol"/>.</returns>
        /// <exception cref="NotSupportedException">The type {property.PropertyType.FullName} of {property.Name}</exception>
        public static TsPropertySymbol LoadFrom(PropertyInfo property, TsSymbolLookup symbolLookup)
        {
            TsPropertyAttribute attribute = property.GetCustomAttribute<TsPropertyAttribute>();

            bool isOptional = GetIsOptional(property, attribute);
            bool isReadOnly = GetIsReadOnly(property, attribute);

            if (!symbolLookup.TryResolveSymbol(property.PropertyType, out TsTypeSymbol type))
            {
                throw new NotSupportedException($"The type {property.PropertyType.FullName} of {property.Name} is not supported.");
            }

            return new TsPropertySymbol(property.Name, type, isOptional, isReadOnly);
        }

        /// <summary>
        /// Determines if the property is optional.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns><c>true</c> if the property is optional, <c>false</c> otherwise.</returns>
        /// <exception cref="NotSupportedException">The value ({userOptions}) specified for {nameof(TsPropertyAttribute.IsOptional)} on the {propertyInfo.Name}</exception>
        private static bool GetIsOptional(PropertyInfo propertyInfo, TsPropertyAttribute attribute)
        {
            TsPropertyValueOptions userOptions = attribute?.IsOptional ?? TsPropertyValueOptions.Auto;

            switch (userOptions)
            {
                case TsPropertyValueOptions.Auto:
                    return propertyInfo.PropertyType.IsClass || Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;

                case TsPropertyValueOptions.No:
                    return false;

                case TsPropertyValueOptions.Yes:
                    return true;

                default:
                    throw new NotSupportedException($"The value ({userOptions}) specified for {nameof(TsPropertyAttribute.IsOptional)} on the {propertyInfo.Name} property is not supported.");
            }
        }

        /// <summary>
        /// Determines if the property is read only.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns><c>true</c> if the property is read only, <c>false</c> otherwise.</returns>
        /// <exception cref="NotSupportedException">The value ({userOptions}) specified for {nameof(TsPropertyAttribute.IsReadOnly)} on the {propertyInfo.Name}</exception>
        private static bool GetIsReadOnly(PropertyInfo propertyInfo, TsPropertyAttribute attribute)
        {
            TsPropertyValueOptions userOptions = attribute?.IsReadOnly ?? TsPropertyValueOptions.Auto;

            switch (userOptions)
            {
                case TsPropertyValueOptions.Auto:
                    return propertyInfo.SetMethod == null;

                case TsPropertyValueOptions.No:
                    return false;

                case TsPropertyValueOptions.Yes:
                    return true;

                default:
                    throw new NotSupportedException($"The value ({userOptions}) specified for {nameof(TsPropertyAttribute.IsReadOnly)} on the {propertyInfo.Name} property is not supported.");
            }
        }
    }
}