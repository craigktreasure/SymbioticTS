using System;

namespace SymbioticTS.Abstractions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TsPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the property is read only.
        /// </summary>
        public TsPropertyValueOptions IsReadOnly { get; set; } = TsPropertyValueOptions.Auto;

        /// <summary>
        /// Gets or sets a value indicating whether or not the property is optional.
        /// </summary>
        public TsPropertyValueOptions IsOptional { get; set; } = TsPropertyValueOptions.Auto;

        /// <summary>
        /// Initializes a new instance of the <see cref="TsPropertyAttribute" /> class.
        /// </summary>
        /// <param name="options">The property options.</param>
        public TsPropertyAttribute()
        {
        }
    }
}