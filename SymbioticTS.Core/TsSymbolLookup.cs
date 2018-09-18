using System;
using System.Collections;
using System.Collections.Generic;

namespace SymbioticTS.Core
{
    internal class TsSymbolLookup
    {
        private readonly IDictionary<Type, TsTypeSymbol> lookup = new Dictionary<Type, TsTypeSymbol>
        {
            [typeof(bool)] = TsTypeSymbol.Boolean,
            [typeof(short)] = TsTypeSymbol.Number,
            [typeof(ushort)] = TsTypeSymbol.Number,
            [typeof(int)] = TsTypeSymbol.Number,
            [typeof(uint)] = TsTypeSymbol.Number,
            [typeof(long)] = TsTypeSymbol.Number,
            [typeof(ulong)] = TsTypeSymbol.Number,
            [typeof(double)] = TsTypeSymbol.Number,
            [typeof(float)] = TsTypeSymbol.Number,
            [typeof(decimal)] = TsTypeSymbol.Number,
            [typeof(string)] = TsTypeSymbol.String,
            [typeof(DateTime)] = TsTypeSymbol.Date,
            [typeof(DateTimeOffset)] = TsTypeSymbol.Date,
        };

        /// <summary>
        /// Gets the number of items contained in the lookup.
        /// </summary>
        /// <value>The count.</value>
        public int Count => this.lookup.Count;

        /// <summary>
        /// Gets an <see cref="ICollection{TsTypeSymbol}"></see> containing the values in the lookup.
        /// </summary>
        /// <value>The values.</value>
        public ICollection<TsTypeSymbol> Symbols => this.lookup.Values;

        /// <summary>
        /// Gets an <see cref="ICollection{Type}"></see> containing the keys of the lookup.
        /// </summary>
        /// <value>The keys.</value>
        public ICollection<Type> Types => this.lookup.Keys;

        /// <summary>
        /// Adds an item to the lookup with the provided key and value.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <param name="symbol">The <see cref="TsTypeSymbol"/>.</param>
        public void Add(Type type, TsTypeSymbol symbol) => this.lookup.Add(type, symbol);

        /// <summary>
        /// Determines whether the lookup contains an element with the specified key.
        /// </summary>
        /// <param name="type">The key to locate in the lookup.</param>
        /// <returns>true if the lookup contains an element with the key; otherwise, false.</returns>
        public bool ContainsType(Type type) => this.lookup.ContainsKey(type);

        /// <summary>
        /// Resolves the symbol for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="TsTypeSymbol"/>.</returns>
        /// <exception cref="KeyNotFoundException">A symbol for the type {type.FullName}</exception>
        public TsTypeSymbol ResolveSymbol(Type type)
        {
            if (!this.TryResolveSymbol(type, out TsTypeSymbol symbol))
            {
                throw new KeyNotFoundException($"A symbol for the type {type.FullName} could not be resolved.");
            }

            return symbol;
        }

        /// <summary>
        /// Tries to resolve the symbol.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="symbol">The symbol.</param>
        /// <returns><c>true</c> if the symbol was resolved, <c>false</c> otherwise.</returns>
        public bool TryResolveSymbol(Type type, out TsTypeSymbol symbol)
        {
            symbol = null;

            if (type.IsArray)
            {
                if (!this.TryResolveSymbol(type.GetElementType(), out TsTypeSymbol elementSymbol))
                {
                    return false;
                }

                symbol = TsTypeSymbol.CreateArraySymbol(elementSymbol);
                return true;
            }

            if (type != typeof(string) && type.IsAssignableTo(typeof(IEnumerable)))
            {
                Type[] genericTypes = type.GetGenericArguments();

                if (!this.TryResolveSymbol(genericTypes[0], out TsTypeSymbol elementSymbol))
                {
                    return false;
                }

                symbol = TsTypeSymbol.CreateArraySymbol(elementSymbol);
                return true;
            }

            return this.lookup.TryGetValue(type.UnwrapNullable(), out symbol);
        }
    }
}