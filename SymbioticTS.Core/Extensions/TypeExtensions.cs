using System;

namespace SymbioticTS
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified <see cref="Type"/> is a .Net Framework <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified <see cref="Type"/> is a .Net Framework <see cref="Type"/>; otherwise, <c>false</c>.</returns>
        public static bool IsNetFramework(this Type type)
        {
            return type.Assembly.IsNetFramework();
        }
    }
}