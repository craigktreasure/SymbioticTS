using System;

namespace SymbioticTS.Abstractions
{
    [Flags]
    public enum PropertyOptions
    {
        /// <summary>
        /// The options are automatically inferred.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// The property is marked as readonly.
        /// </summary>
        ReadOnly,

        /// <summary>
        /// The property is marked as optional.
        /// </summary>
        Optional
    }
}