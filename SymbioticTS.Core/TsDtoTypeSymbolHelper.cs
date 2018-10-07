using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DtoTypeSymbolMap = System.Collections.Generic.Dictionary<SymbioticTS.Core.TsTypeSymbol, SymbioticTS.Core.TsTypeSymbol>;

namespace SymbioticTS.Core
{
    internal class TsDtoTypeSymbolHelper
    {
        private static readonly Dictionary<TsTypeSymbol, TsTypeSymbol> dtoPropertyTypeSymbolMap = new Dictionary<TsTypeSymbol, TsTypeSymbol>
        {
            [TsTypeSymbol.Date] = TsTypeSymbol.String
        };

        private readonly ISymbolLoadOptions options;

        private readonly TsSymbolLookup symbolLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="TsDtoTypeSymbolHelper"/> class.
        /// </summary>
        /// <param name="symbolLookup">The symbol lookup.</param>
        /// <param name="options">The options.</param>
        public TsDtoTypeSymbolHelper(TsSymbolLookup symbolLookup, ISymbolLoadOptions options)
        {
            this.options = options;
            this.symbolLookup = symbolLookup;
        }

        /// <summary>
        /// Determines if the specified <see cref="TsTypeSymbol"/> requires a data transfer
        /// object transformation.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        /// <returns><c>true</c> if a DTO transorm is required, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">typeSymbol</exception>
        public static bool RequiresDtoTransform(TsTypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            if (dtoPropertyTypeSymbolMap.ContainsKey(typeSymbol))
            {
                return true;
            }

            if (typeSymbol.Properties.Any(p => RequiresDtoTransform(p.Type)))
            {
                return true;
            }

            if (typeSymbol.Base != null && RequiresDtoTransform(typeSymbol.Base))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates and configures DTO symbols.
        /// </summary>
        /// <param name="typeSymbols">The type symbols.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="TsTypeSymbol"/>.</returns>
        public IEnumerable<TsTypeSymbol> CreateAndConfigureDtoSymbols(IEnumerable<TsTypeSymbol> typeSymbols)
        {
            DtoTypeSymbolMap dtoSymbolMap = new DtoTypeSymbolMap();

            IEnumerable<TsTypeSymbol> dtoClasses = typeSymbols.Where(s => s.TypeMetadata?.IsDto ?? false);

            foreach (TsTypeSymbol typeSymbol in dtoClasses)
            {
                this.MakeDtoInterfaces(typeSymbol, dtoSymbolMap);
            }

            return dtoSymbolMap.Values;
        }

        private ISet<string> GetInterfacePropertyNames(Type type)
        {
            HashSet<string> interfacePropertyNames = new HashSet<string>();

            foreach (Type interfaceType in type.GetInterfaces())
            {
                interfacePropertyNames.AddRange(
                    interfaceType.GetProperties(
                        BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => p.Name));
            }

            return interfacePropertyNames;
        }

        private TsTypeSymbol GetOrCreateDtoInterface(TsTypeSymbol typeSymbol, DtoTypeSymbolMap dtoSymbolMap)
        {
            TsTypeSymbol unwrappedTypeSymbol = typeSymbol.UnwrapArray(out int rank);

            if (!dtoSymbolMap.TryGetValue(unwrappedTypeSymbol, out TsTypeSymbol dtoInterfaceSymbol))
            {
                dtoInterfaceSymbol = this.MakeAndRegisterDtoInterface(unwrappedTypeSymbol, dtoSymbolMap);
            }

            if (rank > 0)
            {
                return TsTypeSymbol.CreateArraySymbol(dtoInterfaceSymbol, rank);
            }

            return dtoInterfaceSymbol;
        }

        private TsPropertySymbol GetPropertySymbol(PropertyInfo propertyInfo, DtoTypeSymbolMap dtoSymbolMap)
        {
            TsPropertySymbol result = TsPropertySymbol.LoadFrom(propertyInfo, this.symbolLookup, this.options);

            TsTypeSymbol unwrappedTypeSymbol = result.Type.UnwrapArray(out int arrayRank);

            if (dtoPropertyTypeSymbolMap.TryGetValue(unwrappedTypeSymbol, out TsTypeSymbol replacementType))
            {
                // We need a type replacement.
                if (result.Type.IsArray)
                {
                    replacementType = TsTypeSymbol.CreateArraySymbol(replacementType, arrayRank);
                }

                result = new TsPropertySymbol(result.Name, replacementType, result.PropertyMetadata);
            }
            else if ((unwrappedTypeSymbol.IsClass || unwrappedTypeSymbol.IsInterface)
                && this.symbolLookup.ContainsType(unwrappedTypeSymbol.TypeMetadata.Type))
            {
                // We need a DTO Interface for this type.
                TsTypeSymbol dtoInterfaceType = this.GetOrCreateDtoInterface(unwrappedTypeSymbol, dtoSymbolMap);

                if (result.Type.IsArray)
                {
                    dtoInterfaceType = TsTypeSymbol.CreateArraySymbol(dtoInterfaceType, arrayRank);
                }

                result = new TsPropertySymbol(result.Name, dtoInterfaceType, result.PropertyMetadata);
            }

            return result;
        }

        private TsTypeSymbol MakeAndRegisterDtoInterface(TsTypeSymbol typeSymbol, DtoTypeSymbolMap dtoSymbolMap)
        {
            TsTypeSymbol unwrappedTypeSymbol = typeSymbol.UnwrapArray(out int rank);

            Type type = unwrappedTypeSymbol.TypeMetadata.Type;

            string preparedName = unwrappedTypeSymbol.Name;

            if (unwrappedTypeSymbol.IsInterface
                && preparedName.StartsWith("I")
                && preparedName.Length > 1)
            {
                preparedName = preparedName.Substring(1);
            }

            string interfaceName = $"I{preparedName}Dto";

            List<TsTypeSymbol> interfaceTypeSymbols = unwrappedTypeSymbol.Interfaces
                .Select(i => this.GetOrCreateDtoInterface(i, dtoSymbolMap))
                .ToList();

            if (unwrappedTypeSymbol.Base != null)
            {
                // Inject the base DTO interface into the interfaces.
                interfaceTypeSymbols.Insert(0, this.GetOrCreateDtoInterface(unwrappedTypeSymbol.Base, dtoSymbolMap));
            }

            IReadOnlyList<TsPropertySymbol> propertySymbols = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Select(p => this.GetPropertySymbol(p, dtoSymbolMap))
                .Apply();

            if (type.IsClass)
            {
                // We need to remove any properties defined in interfaces
                // to avoid readonly collisions.
                ISet<string> interfacePropertyNames = this.GetInterfacePropertyNames(type);

                propertySymbols = propertySymbols
                    .Where(p => !interfacePropertyNames.Contains(p.PropertyMetadata.Property.Name))
                    .Apply();
            }

            TsTypeSymbol dtoInterfaceSymbol = new TsTypeSymbol(
                interfaceName,
                TsSymbolKind.Interface,
                baseTypeSymbol: null,
                interfaceTypeSymbols,
                propertySymbols,
                typeMetadata: null);

            unwrappedTypeSymbol.SetDtoInterface(dtoInterfaceSymbol);

            dtoSymbolMap.Add(unwrappedTypeSymbol, dtoInterfaceSymbol);

            return dtoInterfaceSymbol;
        }

        private void MakeDtoInterfaces(TsTypeSymbol type, DtoTypeSymbolMap dtoSymbolMap)
        {
            if (!(type.TypeMetadata?.IsDto ?? false))
            {
                throw new ArgumentException("Can't create a DTO from the specified type symbol.", nameof(type));
            }

            this.GetOrCreateDtoInterface(type, dtoSymbolMap);
        }
    }
}