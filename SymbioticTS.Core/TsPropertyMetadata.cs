using SymbioticTS.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class TsPropertyMetadata
    {
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <value>The attribute.</value>
        public TsPropertyAttribute Attribute { get; }

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
        public string Name => this.Property.Name;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsPropertyMetadata" /> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <exception cref="ArgumentNullException">propertyInfo</exception>
        private TsPropertyMetadata(PropertyInfo propertyInfo)
        {
            this.Property = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            this.Attribute = propertyInfo.GetCustomAttribute<TsPropertyAttribute>(true) ?? FindInheritedAttribute(propertyInfo);
            this.IsReadOnly = GetIsReadOnly(propertyInfo, this.Attribute);
            this.IsOptional = GetIsOptional(propertyInfo, this.Attribute);
        }

        /// <summary>
        /// Loads the <see cref="TsPropertyMetadata"/> from the specified <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>A <see cref="TsPropertyMetadata"/>.</returns>
        internal static TsPropertyMetadata LoadFrom(PropertyInfo propertyInfo)
        {
            return new TsPropertyMetadata(propertyInfo);
        }

        /// <summary>
        /// Finds an inherited attribute on interfaces if available.
        /// </summary>
        /// <remarks>
        /// Performs a breadth search to find the first attribute available for the specified property.
        /// </remarks>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns><see cref="TsPropertyAttribute"/> or null.</returns>
        private static TsPropertyAttribute FindInheritedAttribute(PropertyInfo propertyInfo)
        {
            Type type = propertyInfo.DeclaringType;

            IReadOnlyList<Type> interfaces = type.GetInterfaces();
            TsPropertyAttribute attribute = null;

            while (interfaces.Count != 0 && attribute == null)
            {
                foreach (Type @interface in interfaces)
                {
                    PropertyInfo interfaceProperty = @interface.GetProperty(
                        propertyInfo.Name,
                        BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

                    if (interfaceProperty != null)
                    {
                        attribute = interfaceProperty.GetCustomAttribute<TsPropertyAttribute>(true);

                        if (attribute != null)
                        {
                            break;
                        }
                    }
                }

                if (attribute == null)
                {
                    interfaces = interfaces.SelectMany(i => i.GetInterfaces()).Apply();
                }
            }

            return attribute;
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