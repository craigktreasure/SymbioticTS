using System.Collections.Generic;

namespace SymbioticTS.Core.Visitors
{
    internal class TsSymbolImportVisitor : TsTypeSymbolVisitor
    {
        private readonly TsDirectDependencyTypeSymbolVisitor dependencyVisitor = new TsDirectDependencyTypeSymbolVisitor();

        private HashSet<TsTypeSymbol> symbols;

        /// <summary>
        /// Gathers the import symbols that the specified <see cref="TsTypeSymbol"/> requires.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        /// <returns><see cref="IReadOnlyCollection{T}"/> of <see cref="TsTypeSymbol"/>.</returns>
        public IReadOnlyCollection<TsTypeSymbol> GatherImportSymbols(TsTypeSymbol typeSymbol)
        {
            try
            {
                HashSet<TsTypeSymbol> importSymbols = this.dependencyVisitor.GetDependencies(typeSymbol) as HashSet<TsTypeSymbol>;

                if ((typeSymbol.IsInterface || typeSymbol.IsAbstractClass) && typeSymbol.HasDtoInterface)
                {
                    // An interface or abstract class has no need to depend on its own dto symbol interface.
                    importSymbols.Remove(typeSymbol.DtoInterface);
                }

                if (typeSymbol.IsClass && typeSymbol.HasDtoInterface)
                {
                    // Get all of the dto interface's interface properties that require transform.
                    this.symbols = new HashSet<TsTypeSymbol>();
                    foreach (TsPropertySymbol dtoProperty in typeSymbol.Properties)
                    {
                        this.VisitPropertySymbol(dtoProperty);
                    }

                    importSymbols.AddRange(this.symbols);
                }

                return importSymbols;
            }
            finally
            {
                this.symbols = null;
            }
        }

        /// <summary>
        /// Visits the type symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        protected override void VisitTypeSymbol(TsTypeSymbol typeSymbol)
        {
            if (typeSymbol.IsInterface && typeSymbol.HasDtoInterface && typeSymbol.RequiresDtoTransform())
            {
                this.symbols.Add(typeSymbol.DtoInterface);
            }

            base.VisitTypeSymbol(typeSymbol);
        }
    }
}