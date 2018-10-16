using SymbioticTS.Core.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class TsTypeManager
    {
        private readonly TsTypeManagerOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TsTypeManager"/> class.
        /// </summary>
        public TsTypeManager()
            : this(TsTypeManagerOptions.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsTypeManager"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public TsTypeManager(TsTypeManagerOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Discovers the assemblies that may contain relevant <see cref="Type"/> objects.
        /// </summary>
        /// <param name="assembly">The root assembly.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> of <see cref="Assembly"/>.</returns>
        public IReadOnlyList<Assembly> DiscoverAssemblies(Assembly assembly)
        {
            return DiscoverAllSupportingAssemblies(assembly).Distinct().Apply();
        }

        /// <summary>
        /// Discovers relevant <see cref="Type"/> objects from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of <see cref="Type"/>.</returns>
        public IReadOnlyList<Type> DiscoverTypes(IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(a => this.DiscoverTypes(a))
                .Distinct()
                .Apply();
        }

        /// <summary>
        /// Discovers relevant <see cref="Type"/> objects from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of <see cref="Type"/>.</returns>
        public IReadOnlyList<Type> DiscoverTypes(Assembly assembly)
        {
            return this.DiscoverTypes(assembly.DefinedTypes);
        }

        /// <summary>
        /// Discovers relavant <see cref="Type" /> objects from the specified list of types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>An <see cref="IReadOnlyList{T}" /> of <see cref="Type" />.</returns>
        public IReadOnlyList<Type> DiscoverTypes(params Type[] types)
        {
            return this.DiscoverTypes(types.AsEnumerable());
        }

        /// <summary>
        /// Discovers relavant <see cref="Type" /> objects from the specified list of types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>An <see cref="IReadOnlyList{T}" /> of <see cref="Type" />.</returns>
        public IReadOnlyList<Type> DiscoverTypes(IEnumerable<Type> types)
        {
            IEnumerable<Type> explicitlyAttributedTypes = types
                .Where(IsTypeScriptType);

            TsFullDependencyVisitor visitor = new TsFullDependencyVisitor();

            return visitor.GetAllTypes(explicitlyAttributedTypes);
        }

        /// <summary>
        /// Resolves the type symbols for the specified <see cref="Type"/> objects.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of <see cref="TsTypeSymbol"/>.</returns>
        public IReadOnlyList<TsTypeSymbol> ResolveTypeSymbols(IEnumerable<Type> types)
        {
            List<TsTypeSymbol> results = new List<TsTypeSymbol>();
            TsSymbolLookup symbolLookup = new TsSymbolLookup();
            TsDtoTypeSymbolHelper dtoHelper = new TsDtoTypeSymbolHelper(symbolLookup, this.options);

            TsDependencySortVisitor sortVisitor = new TsDependencySortVisitor();
            IReadOnlyList<Type> sortedTypes = sortVisitor.Sort(types);

            foreach (Type type in sortedTypes)
            {
                TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, symbolLookup, this.options);

                symbolLookup.Add(type, symbol);
                results.Add(symbol);
            }

            results.AddRange(dtoHelper.CreateAndConfigureDtoSymbols(results));

            return results;
        }

        private static IEnumerable<Assembly> DiscoverAllSupportingAssemblies(Assembly assembly)
        {
            if (assembly.IsNetFramework())
            {
                yield break;
            }

            if (HasSymbioticTSAbstractionsReference(assembly))
            {
                yield return assembly;
            }

            foreach (Assembly referencedAssembly in assembly.GetReferencedAssemblies()
                .SelectMany(a => DiscoverAllSupportingAssemblies(Assembly.Load(a))))
            {
                yield return referencedAssembly;
            }
        }

        private static bool HasSymbioticTSAbstractionsReference(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies().Any(name => name.Name == "SymbioticTS.Abstractions");
        }

        private static bool IsTypeScriptType(Type type)
        {
            return TsTypeMetadata.LoadFrom(type).ExplicitOptIn;
        }
    }
}