using System;
using System.Collections.Generic;
using System.Linq;

namespace SymbioticTS
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Gets the enumeration items.
        /// </summary>
        /// <param name="enumType">The type of the enum.</param>
        /// <returns>The enumeration items.</returns>
        /// <remarks>This method assumes that all values can be converted to an integer otherwise it throws an exception.</remarks>
        /// <exception cref="ArgumentNullException">enumType</exception>
        /// <exception cref="ArgumentException">The specified type {enumType.FullName} - enumType</exception>
        public static IEnumerable<(string name, int value)> GetEnumItems(this Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumType));
            }

            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"The specified type {enumType.FullName} is not an enum.", nameof(enumType));
            }

            return GetEnumItemsIterator();

            IEnumerable<(string name, int value)> GetEnumItemsIterator()
            {
                string[] names = Enum.GetNames(enumType);
                IReadOnlyList<object> values = Enum.GetValues(enumType).Cast<object>().Apply();
                for (int i = 0; i < names.Length; i++)
                {
                    yield return (names[i], Convert.ToInt32(values[i]));
                }
            }
        }

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