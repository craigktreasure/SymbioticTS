using System;

namespace SymbioticTS.Abstractions
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class TsEnumAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the TypeScript object generated.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating that the enum should be generated as a constant enum.
        /// </summary>
        /// <value><c>true</c> to generate a constant enum; otherwise, <c>false</c>.</value>
        public bool AsConstant { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsEnumAttribute"/> class.
        /// </summary>
        /// <param name="name">The enum name. Auto generated if null.</param>
        /// <param name="asConstant">if set to <c>true</c>, generate a constant enum.</param>
        public TsEnumAttribute(string name = null, bool asConstant = false)
        {
            this.Name = name;
            this.AsConstant = asConstant;
        }
    }
}