using SymbioticTS.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class TsTypeMetadata
    {
        /// <summary>
        /// Gets the attribute, if available.
        /// </summary>
        /// <value>The attribute.</value>
        public Attribute Attribute { get; }

        /// <summary>
        /// Gets a value indicating whether the type was explicitly opted into.
        /// </summary>
        /// <value><c>true</c> if explicitly opted into; otherwise, <c>false</c>.</value>
        public bool ExplicitOptIn => this.Attribute != null;

        /// <summary>
        /// Gets a value indicating whether this <see cref="TsTypeMetadata"/> is to be flattened.
        /// </summary>
        /// <value><c>true</c> to flatten; otherwise, <c>false</c>.</value>
        public bool Flatten { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a constant enumeration.
        /// </summary>
        /// <value><c>true</c> if this instance is a constant enumeration; otherwise, <c>false</c>.</value>
        public bool IsConstantEnum => this.IsEnum && ((this.Attribute as TsEnumAttribute)?.AsConstant ?? false);

        /// <summary>
        /// Gets a value indicating whether this instance is an enumeration.
        /// </summary>
        /// <value><c>true</c> if this instance is an enumeration; otherwise, <c>false</c>.</value>
        public bool IsEnum => this.Type.IsEnum;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsTypeMetadata"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attribute">The attribute.</param>
        internal TsTypeMetadata(Type type, Attribute attribute)
        {
            this.Type = type;
            this.Attribute = attribute;
            this.Flatten = ShouldFlattenInheritance(attribute);
            this.Name = GetTypeName(type, attribute);
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/>.</returns>
        public IEnumerable<PropertyInfo> GetProperties()
        {
            return GetProperties(this.Type, this.Flatten);
        }

        /// <summary>
        /// Loads a <see cref="TsTypeMetadata"/> from the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="TsTypeMetadata"/>.</returns>
        internal static TsTypeMetadata LoadFrom(Type type)
        {
            Attribute attribute = GetAttribute(type);

            return new TsTypeMetadata(type, attribute);
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Attribute.</returns>
        /// <exception cref="NotSupportedException">The type {type.FullName}</exception>
        private static Attribute GetAttribute(Type type)
        {
            IReadOnlyList<Attribute> attributes = GetAttributes(type).Apply();

            if (attributes.Count > 1)
            {
                throw new NotSupportedException($"The type {type.FullName} has more than one Ts attribute applied, which is not allowed.");
            }

            return attributes.FirstOrDefault();
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;Attribute&gt;.</returns>
        private static IEnumerable<Attribute> GetAttributes(Type type)
        {
            return new Attribute[]
            {
                type.GetTypeInfo().GetCustomAttribute<TsClassAttribute>(),
                type.GetTypeInfo().GetCustomAttribute<TsDtoAttribute>(),
                type.GetTypeInfo().GetCustomAttribute<TsEnumAttribute>(),
                type.GetTypeInfo().GetCustomAttribute<TsInterfaceAttribute>(),
            }.Where(a => a != null);
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flat">if set to <c>true</c> [flat].</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/>.</returns>
        private static IEnumerable<PropertyInfo> GetProperties(Type type, bool flat)
        {
            BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;

            if (!flat)
            {
                propertyBindingFlags |= BindingFlags.DeclaredOnly;
            }

            return type.GetProperties(propertyBindingFlags);
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>A <see cref="String"/>.</returns>
        /// <exception cref="InvalidOperationException">The name value applied to the type {type.FullName}</exception>
        private static string GetTypeName(Type type, Attribute attribute)
        {
            if (attribute is ITsNameableAttribute nameableAttribute)
            {
                if (nameableAttribute.Name != null && string.IsNullOrWhiteSpace(nameableAttribute.Name))
                {
                    throw new InvalidOperationException($"The name value applied to the type {type.FullName} is not valid.");
                }

                return nameableAttribute.Name ?? type.Name;
            }

            return type.Name;
        }

        /// <summary>
        /// Should flatten inheritance.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns><c>true</c> if the attribute indicates that the object should be flattened, <c>false</c> otherwise.</returns>
        private static bool ShouldFlattenInheritance(Attribute attribute)
        {
            if (attribute is ITsFlattenableAttribute flattenableAttribute)
            {
                return flattenableAttribute.FlattenInheritance;
            }

            return false;
        }
    }
}