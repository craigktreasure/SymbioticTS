using System;
using System.Collections.Generic;

namespace SymbioticTS.Core
{
    internal static class TypeScriptLanguage
    {
        /// <summary>
        /// The reserved keywords.
        /// </summary>
        /// <remarks>
        /// Sourced from:
        ///     https://github.com/Microsoft/TypeScript/issues/2536
        ///     https://github.com/Microsoft/TypeScript-Handbook/blob/8b54c8ddc6bd82556aa2b187583b15497d7e93b5/pages/Keywords.md
        ///     https://github.com/Microsoft/TypeScript-Handbook/blob/d1387e8475c98b202ef82a9de4401477c831b948/pages/Keywords.md
        /// </remarks>
        private static readonly HashSet<string> reservedKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            // Reserved Keywords
            "break",
            "case",
            "catch",
            "class",
            "const",
            "continue",
            "debugger",
            "default",
            "delete",
            "do",
            "else",
            "enum",
            "export",
            "extends",
            "false",
            "finally",
            "for",
            "function",
            "if",
            "import",
            "in",
            "instanceof",
            "new",
            "null",
            "return",
            "super",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "var",
            "void",
            "while",
            "with",

            // Strict Mode Reserved Words
            "as",
            "implements",
            "interface",
            "let",
            "package",
            "private",
            "protected",
            "public",
            "static",
            "yield",

            // Contextual Keywords
            "any",
            "boolean",
            "constructor",
            "declare",
            "get",
            "module",
            "require",
            "number",
            "set",
            "string",
            "symbol",
            "type",
            "from",
            "of",
        };

        /// <summary>
        /// Determines whether the specified value is a reserved TypeScript keyword.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is a reserved TypeScript keyword; otherwise, <c>false</c>.</returns>
        public static bool IsReservedKeyword(string value)
        {
            return reservedKeywords.Contains(value);
        }
    }
}