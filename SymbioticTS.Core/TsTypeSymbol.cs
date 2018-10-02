using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SymbioticTS.Core
{
    [DebuggerDisplay("{Name}")]
    internal class TsTypeSymbol
    {
        /// <summary>
        /// Gets the any <see cref="TsTypeSymbol"/>.
        /// </summary>
        public static TsTypeSymbol Any { get; } = new TsTypeSymbol("any", TsSymbolKind.Any);

        /// <summary>
        /// Gets the boolean <see cref="TsTypeSymbol"/>.
        /// </summary>
        public static TsTypeSymbol Boolean { get; } = new TsTypeSymbol("boolean", TsSymbolKind.Boolean);

        /// <summary>
        /// Gets the Date <see cref="TsTypeSymbol"/>.
        /// </summary>
        public static TsTypeSymbol Date { get; } = new TsTypeSymbol("Date", TsSymbolKind.Date);

        /// <summary>
        /// Gets the number <see cref="TsTypeSymbol"/>.
        /// </summary>
        public static TsTypeSymbol Number { get; } = new TsTypeSymbol("number", TsSymbolKind.Number);

        /// <summary>
        /// Gets the string <see cref="TsTypeSymbol"/>.
        /// </summary>
        public static TsTypeSymbol String { get; } = new TsTypeSymbol("string", TsSymbolKind.String);

        /// <summary>
        /// Gets the unknown <see cref="TsTypeSymbol"/>.
        /// </summary>
        public static TsTypeSymbol Unknown { get; } = new TsTypeSymbol("unknown", TsSymbolKind.Unknown);

        /// <summary>
        /// Gets the base <see cref="TsTypeSymbol"/>.
        /// </summary>
        /// <value>The base.</value>
        public TsTypeSymbol Base { get; }

        /// <summary>
        /// Gets the data transfer object interface, if any, associated with this symbol.
        /// </summary>
        public TsTypeSymbol DtoInterface { get; private set; }

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
        /// Gets a value indicating whether this symbol has a DTO interface.
        /// </summary>
        public bool HasDtoInterface => this.DtoInterface != null;

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <value>The interfaces.</value>
        public IReadOnlyList<TsTypeSymbol> Interfaces { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an abstract class.
        /// </summary>
        public bool IsAbstractClass => this.IsClass && (this.TypeMetadata?.Type?.IsAbstract ?? false);

        /// <summary>
        /// Gets a value indicating whether this instance is an array.
        /// </summary>
        /// <value><c>true</c> if this instance is array; otherwise, <c>false</c>.</value>
        public bool IsArray => this.Kind == TsSymbolKind.Array;

        /// <summary>
        /// Gets a value indicating whether this instance is a class.
        /// </summary>
        /// <value><c>true</c> if this instance is class; otherwise, <c>false</c>.</value>
        public bool IsClass => this.Kind == TsSymbolKind.Class;

        /// <summary>
        /// Gets a value indicating whether this instance is a constant enumeration.
        /// </summary>
        /// <value><c>true</c> if this instance is a constant enumeration; otherwise, <c>false</c>.</value>
        public bool IsConstantEnum => this.TypeMetadata?.IsConstantEnum ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is an enumeration.
        /// </summary>
        /// <value><c>true</c> if this instance is an enumeration; otherwise, <c>false</c>.</value>
        public bool IsEnum => this.Kind == TsSymbolKind.Enum;

        /// <summary>
        /// Gets a value indicating whether this instance is an interface.
        /// </summary>
        /// <value><c>true</c> if this instance is interface; otherwise, <c>false</c>.</value>
        public bool IsInterface => this.Kind == TsSymbolKind.Interface;

        /// <summary>
        /// Gets a value indicating whether this symbol is primitive.
        /// </summary>
        /// <value><c>true</c> if this symbol is primitive; otherwise, <c>false</c>.</value>
        public bool IsPrimitive => this.IsArray
            || this == Boolean
            || this == Number
            || this == Date
            || this == String;

        /// <summary>
        /// Gets the symbol kind.
        /// </summary>
        public TsSymbolKind Kind { get; }

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
        /// Gets the type metadata.
        /// </summary>
        /// <value>The type metadata.</value>
        internal TsTypeMetadata TypeMetadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsTypeSymbol"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="symbolKind">The kind of symbol.</param>
        internal TsTypeSymbol(string name, TsSymbolKind symbolKind)
            : this(name, symbolKind, null, new TsTypeSymbol[0], new TsPropertySymbol[0], null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsTypeSymbol" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="symbolKind">The kind of symbol.</param>
        /// <param name="baseTypeSymbol">The base type symbol.</param>
        /// <param name="interfaceTypeSymbols">The interface type symbols.</param>
        /// <param name="propertySymbols">The property symbols.</param>
        /// <param name="typeMetadata">The type metadata.</param>
        internal TsTypeSymbol(string name, TsSymbolKind symbolKind, TsTypeSymbol baseTypeSymbol,
            IReadOnlyList<TsTypeSymbol> interfaceTypeSymbols, IReadOnlyList<TsPropertySymbol> propertySymbols,
            TsTypeMetadata typeMetadata)
        {
            this.Name = name;
            this.Kind = symbolKind;
            this.Base = baseTypeSymbol;
            this.Interfaces = interfaceTypeSymbols;
            this.Properties = propertySymbols;
            this.TypeMetadata = typeMetadata;
        }

        /// <summary>
        /// Gets the enumeration item symbols.
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="TsEnumItemSymbol"/>.</returns>
        /// <exception cref="InvalidOperationException">The type {this.Name}</exception>
        public IEnumerable<TsEnumItemSymbol> GetEnumItemSymbols()
        {
            if (!this.IsEnum)
            {
                throw new InvalidOperationException($"The type {this.Name} is not an enumeration.");
            }

            return this.TypeMetadata.Type.GetEnumItems().Select(i => new TsEnumItemSymbol(i.name, i.value));
        }

        /// <summary>
        /// Sets the data transfer object interface symbol.
        /// </summary>
        /// <param name="dtoInterfaceSymbol">The dto interface symbol.</param>
        public void SetDtoInterface(TsTypeSymbol dtoInterfaceSymbol)
        {
            this.DtoInterface = dtoInterfaceSymbol;
        }

        /// <summary>
        /// Creates an array symbol.
        /// </summary>
        /// <param name="elementType">The type of the element.</param>
        /// <returns>A <see cref="TsTypeSymbol"/>.</returns>
        internal static TsTypeSymbol CreateArraySymbol(TsTypeSymbol elementType)
        {
            return new TsTypeSymbol($"{elementType.Name}[]", TsSymbolKind.Array)
            {
                ElementType = elementType
            };
        }

        /// <summary>
        /// Creates an array symbol.
        /// </summary>
        /// <param name="elementType">The type of the element.</param>
        /// <param name="rank">The rank.</param>
        /// <returns>A <see cref="TsTypeSymbol" />.</returns>
        /// <exception cref="ArgumentException">The rank must be greater than 0.</exception>
        internal static TsTypeSymbol CreateArraySymbol(TsTypeSymbol elementType, int rank)
        {
            if (rank <= 0)
            {
                throw new ArgumentException("The rank must be greater than 0.");
            }

            if (rank == 1)
            {
                return CreateArraySymbol(elementType);
            }

            TsTypeSymbol result = elementType;

            for (int i = 0; i < rank; i++)
            {
                result = CreateArraySymbol(result);
            }

            return result;
        }

        /// <summary>
        /// Loads a <see cref="TsTypeSymbol" /> from the specified <see cref="Kind" /> using the <see cref="TsSymbolLookup" />.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="TsTypeSymbol" />.</returns>
        internal static TsTypeSymbol LoadFrom(Type type, TsSymbolLookup symbolLookup, ISymbolLoadOptions options)
        {
            TsTypeMetadata typeMetadata = TsTypeMetadata.LoadFrom(type);

            return LoadFrom(typeMetadata, symbolLookup, options);
        }

        /// <summary>
        /// Loads a <see cref="TsTypeSymbol" /> from the specified <see cref="TsTypeMetadata" /> using the <see cref="TsSymbolLookup" />.
        /// </summary>
        /// <param name="typeMetadata">The type metadata.</param>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="TsTypeSymbol" />.</returns>
        internal static TsTypeSymbol LoadFrom(TsTypeMetadata typeMetadata, TsSymbolLookup symbolLookup, ISymbolLoadOptions options)
        {
            if (typeMetadata.Type.IsGenericType)
            {
                throw new NotSupportedException("Generic types are not currently supported.");
            }

            TsTypeSymbol baseTypeSymbol = null;
            if (typeMetadata.Type.IsClass
                && !typeMetadata.Flatten
                && typeMetadata.Type.BaseType != null
                && typeMetadata.Type.BaseType != typeof(object))
            {
                baseTypeSymbol = symbolLookup.ResolveSymbol(typeMetadata.Type.BaseType);
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
                propertySymbols.Add(TsPropertySymbol.LoadFrom(property, symbolLookup, options));
            }

            TsTypeSymbol symbol = new TsTypeSymbol
            (
                name: typeMetadata.Name,
                symbolKind: GetSymbolKind(typeMetadata),
                baseTypeSymbol,
                interfaceTypeSymbols,
                propertySymbols,
                typeMetadata
            );

            return symbol;
        }

        /// <summary>
        /// Gets the <see cref="TsSymbolKind"/> of the <see cref="TsTypeMetadata"/>.
        /// </summary>
        /// <param name="typeMetadata">The type metadata.</param>
        /// <returns>A <see cref="TsSymbolKind" />.</returns>
        private static TsSymbolKind GetSymbolKind(TsTypeMetadata typeMetadata)
        {
            return GetSymbolKind(typeMetadata.Type);
        }

        /// <summary>
        /// Gets the <see cref="TsSymbolKind"/> of the <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="TsSymbolKind"/>.</returns>
        private static TsSymbolKind GetSymbolKind(Type type)
        {
            if (type.IsClass)
            {
                return TsSymbolKind.Class;
            }
            else if (type.IsEnum)
            {
                return TsSymbolKind.Enum;
            }
            else if (type.IsInterface)
            {
                return TsSymbolKind.Interface;
            }
            else if (type.IsStruct() && !type.IsPrimitive)
            {
                return TsSymbolKind.Class;
            }
            else
            {
                throw new InvalidOperationException($"Could not determine the {nameof(TsSymbolKind)} for {type.FullName}.");
            }
        }
    }

    internal class TsTypeSymbolComparer : IEqualityComparer<TsTypeSymbol>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(TsTypeSymbol x, TsTypeSymbol y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.TypeMetadata != null && y.TypeMetadata != null)
            {
                return x.TypeMetadata.Type == y.TypeMetadata.Type;
            }

            if (x.TypeMetadata == null && y.TypeMetadata == null)
            {
                return x.Name == y.Name
                    && x.Kind == y.Kind;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(TsTypeSymbol obj)
        {
            return obj.TypeMetadata?.Type.GetHashCode() ?? (obj.Name, obj.Kind).GetHashCode();
        }
    }
}