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
            "b03f5f7f11d50a3a", // System.Runtime
            "adb9793829ddae60", // Many ASP.NET Core assemblies
        };

        /// <summary>
        /// Determines whether the specified <see cref="Assembly"/> is a .Net Framework <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if the specified <see cref="Assembly"/> is a .Net Framework <see cref="Assembly"/>; otherwise, <c>false</c>.</returns>
        public static bool IsNetFramework(this Assembly assembly)
        {
            byte[] tokenBytes = assembly.GetName().GetPublicKeyToken();

            return IsNetFrameworkPublicKeyToken(tokenBytes);
        }

        /// <summary>
        /// Determines whether the specified <see cref="AssemblyName"/> is a .Net Framework <see cref="AssemblyName"/>.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="AssemblyName"/> is a .Net Framework <see cref="AssemblyName"/>; otherwise, <c>false</c>.</returns>
        public static bool IsNetFramework(this AssemblyName assemblyName)
        {
            byte[] tokenBytes = assemblyName.GetPublicKeyToken();

            return IsNetFrameworkPublicKeyToken(tokenBytes);
        }

        private static bool IsNetFrameworkPublicKeyToken(byte[] publicKeyToken)
        {
            if (publicKeyToken == null || publicKeyToken.Length == 0)
            {
                return false;
            }

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < publicKeyToken.Length; i++)
            {
                stringBuilder.AppendFormat("{0:x2}", publicKeyToken[i]);
            }

            string token = stringBuilder.ToString();

            return netFrameworkPublicKeyTokens.Contains(token);
        }
    }
}
