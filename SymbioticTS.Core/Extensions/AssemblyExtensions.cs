using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SymbioticTS
{
    internal static class AssemblyExtensions
    {
        // Approach is documented here: https://stackoverflow.com/questions/3174921/how-do-i-determine-if-system-type-is-a-custom-type-or-a-framework-type

        private readonly static HashSet<string> netFrameworkPublicKeyTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "b77a5c561934e089", // mscorlib
            "7cec85d7bea7798e", // System.Private.CorLib
            "cc7b13ffcd2ddd51", // netstandard
        };

        /// <summary>
        /// Determines whether the specified <see cref="Assembly"/> is a .Net Framework <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if the specified <see cref="Assembly"/> is a .Net Framework <see cref="Assembly"/>; otherwise, <c>false</c>.</returns>
        public static bool IsNetFramework(this Assembly assembly)
        {
            byte[] tokenBytes = assembly.GetName().GetPublicKeyToken();

            if (tokenBytes == null || tokenBytes.Length == 0)
            {
                return false;
            }

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < tokenBytes.Length; i++)
            {
                stringBuilder.AppendFormat("{0:x2}", tokenBytes[i]);
            }

            string token = stringBuilder.ToString();

            return netFrameworkPublicKeyTokens.Contains(token);
        }
    }
}
