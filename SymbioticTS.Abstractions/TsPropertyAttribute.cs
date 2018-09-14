using System;

namespace SymbioticTS.Abstractions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TsPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether the property should be optional.
        /// </summary>
        /// <value><c>true</c> if optional; otherwise, <c>false</c>.</value>
        public bool AsOptional { get; }

        /// <summary>
        /// Gets a value indicating whether the property should be read only.
        /// </summary>
        /// <value><c>true</c> if read only; otherwise, <c>false</c>.</value>
        public bool AsReadOnly { get; }

        /// <summary>
        /// Gets the name of the TypeScript object generated.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsPropertyAttribute"/> class.
        /// </summary>
        /// <param name="name">The property name. Auto generated if null.</param>
        /// <param name="asReadOnly">Generate the property as 'readonly' if set to <c>true</c>.</param>
        /// <param name="asOptional">Generate the property as optional if set to <c>true</c>.</param>
        public TsPropertyAttribute(string name = null, bool asReadOnly = false, bool asOptional = false)
        {
            this.Name = name;
            this.AsReadOnly = asReadOnly;
            this.AsOptional = asOptional;
        }
    }
}