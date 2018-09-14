using System;

namespace SymbioticTS.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TsClassAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the TypeScript object generated.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating that the generated object will
        /// include all properties, but will not inherit from any objects.
        /// </summary>
        /// <value><c>true</c> for flattened inheritance; otherwise, <c>false</c>.</value>
        public bool FlattenInheritance { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsClassAttribute"/> class.
        /// </summary>
        /// <param name="name">The class name. Auto generated if null.</param>
        /// <param name="flattenInheritance">if set to <c>true</c>, flatten inheritance.</param>
        public TsClassAttribute(string name = null, bool flattenInheritance = false)
        {
            this.Name = name;
            this.FlattenInheritance = flattenInheritance;
        }
    }
}