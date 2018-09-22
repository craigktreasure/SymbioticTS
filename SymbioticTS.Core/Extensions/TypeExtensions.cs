using System;

namespace SymbioticTS
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the current type can be assigned to an instance
        /// of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="b">The type to compare with the current type.</param>
        /// <returns><c>true</c> if the condition is true; otherwise, <c>false</c>.</returns>
        public static bool IsAssignableTo(this Type type, Type b)
        {
            return b.IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type"/> is a .Net Framework <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified <see cref="Type"/> is a .Net Framework <see cref="Type"/>; otherwise, <c>false</c>.</returns>
        public static bool IsNetFramework(this Type type)
        {
            return type.Assembly.IsNetFramework() || type.IsArray;
        }

        /// <summary>
        /// Determines whether the specified type is structure.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is structure; otherwise, <c>false</c>.</returns>
        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsEnum;
        }

        /// <summary>
        /// Unwraps a <see cref="Nullable{T}"/> if provided.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A non-nullable <see cref="Type"/>.</returns>
        public static Type UnwrapNullable(this Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
            {
                return underlyingType.UnwrapNullable();
            }

            return type;
        }
    }
}