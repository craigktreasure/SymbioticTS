using System;
using System.Collections.Generic;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class TsTypeSymbol
    {
        /// <summary>
        /// Gets the boolean <see cref="TsTypeSymbol"/>.
        /// </summary>
        /// <value>The boolean.</value>
        public static TsTypeSymbol Boolean { get; } = new TsTypeSymbol("boolean", TsSymbolType.Boolean);

        /// <summary>
        /// Gets the Date <see cref="TsTypeSymbol"/>.
        /// </summary>
        /// <value>The date.</value>
        public static TsTypeSymbol Date { get; } = new TsTypeSymbol("Date", TsSymbolType.Date);

        /// <summary>
        /// Gets the number <see cref="TsTypeSymbol"/>.
        /// </summary>
        /// <value>The number.</value>
        public static TsTypeSymbol Number { get; } = new TsTypeSymbol("number", TsSymbolType.Number);

        /// <summary>
        /// Gets the string <see cref="TsTypeSymbol"/>.
        /// </summary>
        /// <value>The string.</value>
        public static TsTypeSymbol String { get; } = new TsTypeSymbol("string", TsSymbolType.String);

        /// <summary>
        /// Gets the base <see cref="TsTypeSymbol"/>.
        /// </summary>
        /// <value>The base.</value>
        public TsTypeSymbol Base { get; }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>The type of the element.</value>
        public TsTypeSymbol ElementType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the type was explicitly opted into.
        /// </summary>
        /// <value><c>true</c> if explicitly opted into; otherwise, <c>false</c>.</value>
        public bool ExplicitOptIn => this.TypeMetadata?.ExplicitOptIn ?? false;

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <value>The interfaces.</value>
        public IReadOnlyList<TsTypeSymbol> Interfaces { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an array.
        /// </summary>
        /// <value><c>true</c> if this instance is array; otherwise, <c>false</c>.</value>
        public bool IsArray => this.Type == TsSymbolType.Array;

        /// <summary>
        /// Gets a value indicating whether this instance is a class.
        /// </summary>
        /// <value><c>true</c> if this instance is class; otherwise, <c>false</c>.</value>
        public bool IsClass => this.Type == TsSymbolType.Class;

        /// <summary>
        /// Gets a value indicating whether this instance is an enum.
        /// </summary>
        /// <value><c>true</c> if this instance is enum; otherwise, <c>false</c>.</value>
        public bool IsEnum => this.Type == TsSymbolType.Enum;

        /// <summary>
        /// Gets a value indicating whether this instance is an interface.
        /// </summary>
        /// <value><c>true</c> if this instance is interface; otherwise, <c>false</c>.</value>
        public bool IsInterface => this.Type == TsSymbolType.Interface;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public IReadOnlyList<TsPropertySymbol> Properties { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public TsSymbolType Type { get; }

        /// <summary>
        /// Gets the type metadata.
        /// </summary>
        /// <value>The type metadata.</value>
        internal TsTypeMetadata TypeMetadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsTypeSymbol"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="symbolType">Type type of the symbol.</param>
        internal TsTypeSymbol(string name, TsSymbolType symbolType)
            : this(name, symbolType, null, new TsTypeSymbol[0], new TsPropertySymbol[0], null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsTypeSymbol" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="symbolType">The type of the symbol.</param>
        /// <param name="baseTypeSymbol">The base type symbol.</param>
        /// <param name="interfaceTypeSymbols">The interface type symbols.</param>
        /// <param name="propertySymbols">The property symbols.</param>
        /// <param name="typeMetadata">The type metadata.</param>
        internal TsTypeSymbol(string name, TsSymbolType symbolType, TsTypeSymbol baseTypeSymbol,
            IReadOnlyList<TsTypeSymbol> interfaceTypeSymbols, IReadOnlyList<TsPropertySymbol> propertySymbols,
            TsTypeMetadata typeMetadata)
        {
            this.Name = name;
            this.Type = symbolType;
            this.Base = baseTypeSymbol;
            this.Interfaces = interfaceTypeSymbols;
            this.Properties = propertySymbols;
            this.TypeMetadata = typeMetadata;
        }

        /// <summary>
        /// Creates an array symbol.
        /// </summary>
        /// <param name="elementType">The type of the element.</param>
        /// <returns>A <see cref="TsTypeSymbol"/>.</returns>
        internal static TsTypeSymbol CreateArraySymbol(TsTypeSymbol elementType)
        {
            return new TsTypeSymbol(nameof(TsSymbolType.Array), TsSymbolType.Array)
            {
                ElementType = elementType
            };
        }

        /// <summary>
        /// Loads a <see cref="TsTypeSymbol"/> from the specified <see cref="Type"/> using the <see cref="TsSymbolLookup"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <returns>A <see cref="TsTypeSymbol"/>.</returns>
        internal static TsTypeSymbol LoadFrom(Type type, TsSymbolLookup symbolLookup)
        {
            TsTypeMetadata typeMetadata = TsTypeMetadata.LoadFrom(type);

            return LoadFrom(typeMetadata, symbolLookup);
        }

        /// <summary>
        /// Loads a <see cref="TsTypeSymbol"/> from the specified <see cref="TsTypeMetadata"/> using the <see cref="TsSymbolLookup"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <returns>A <see cref="TsTypeSymbol"/>.</returns>
        internal static TsTypeSymbol LoadFrom(TsTypeMetadata typeMetadata, TsSymbolLookup symbolLookup)
        {
            TsTypeSymbol baseTypeSymbol = null;
            if (!typeMetadata.Flatten && typeMetadata.Type.BaseType != null)
            {
                symbolLookup.TryResolveSymbol(typeMetadata.Type.BaseType, out baseTypeSymbol);
            }

            List<TsTypeSymbol> interfaceTypeSymbols = new List<TsTypeSymbol>();
            if (!typeMetadata.Flatten)
            {
                foreach (Type interfaceType in typeMetadata.Type.GetInterfaces())
                {
                    if (symbolLookup.TryResolveSymbol(interfaceType, out TsTypeSymbol interfaceTypeSymbol))
                    {
                        interfaceTypeSymbols.Add(interfaceTypeSymbol);
                    }
                }
            }

            List<TsPropertySymbol> propertySymbols = new List<TsPropertySymbol>();
            foreach (PropertyInfo property in typeMetadata.GetProperties())
            {
                propertySymbols.Add(TsPropertySymbol.LoadFrom(property, symbolLookup));
            }

            TsTypeSymbol symbol = new TsTypeSymbol
            (
                name: typeMetadata.Name,
                symbolType: GetSymbolType(typeMetadata),
                baseTypeSymbol,
                interfaceTypeSymbols,
                propertySymbols,
                typeMetadata
            );

            return symbol;
        }

        /// <summary>
        /// Gets the type of the symbol.
        /// </summary>
        /// <param name="typeMetadata">The type metadata.</param>
        /// <returns>A <see cref="TsSymbolType" />.</returns>
        /// <exception cref="InvalidOperationException">Could not determine the type of {type.FullName}</exception>
        private static TsSymbolType GetSymbolType(TsTypeMetadata typeMetadata)
        {
            return GetSymbolType(typeMetadata.Type);
        }

        /// <summary>
        /// Gets the type of the symbol.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="TsSymbolType"/>.</returns>
        /// <exception cref="InvalidOperationException">Could not determine the type of {type.FullName}</exception>
        private static TsSymbolType GetSymbolType(Type type)
        {
            if (type.IsClass)
            {
                return TsSymbolType.Class;
            }
            else if (type.IsEnum)
            {
                return TsSymbolType.Enum;
            }
            else if (type.IsInterface)
            {
                return TsSymbolType.Interface;
            }
            else if (type.IsStruct() && !type.IsPrimitive)
            {
                return TsSymbolType.Class;
            }
            else
            {
                throw new InvalidOperationException($"Could not determine the type of {type.FullName}.");
            }
        }
    }
}