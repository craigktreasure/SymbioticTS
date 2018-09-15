using System;

namespace SymbioticTS.Abstractions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TsPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets the property options.
        /// </summary>
        /// <value>The options.</value>
        public PropertyOptions Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsPropertyAttribute" /> class.
        /// </summary>
        /// <param name="options">The property options.</param>
        public TsPropertyAttribute(PropertyOptions options = PropertyOptions.Auto)
        {
            this.Options = options;
        }
    }
}