using System;

namespace SymbioticTS.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TsDtoAttribute : Attribute
    {
        /// <summary>
        /// Gets the names of the TypeScript objects generated.
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
        /// Initializes a new instance of the <see cref="TsDtoAttribute"/> class.
        /// </summary>
        /// <param name="name">The DTO name. Auto generated if null.</param>
        /// <param name="flattenInheritance">if set to <c>true</c>, flatten inheritance.</param>
        public TsDtoAttribute(string name = null, bool flattenInheritance = false)
        {
            this.Name = name;
            this.FlattenInheritance = flattenInheritance;
        }
    }
}
