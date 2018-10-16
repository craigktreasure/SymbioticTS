using System.Collections.Generic;

namespace SymbioticTS.Core.Visitors
{
    internal class TsDirectDependencyTypeSymbolVisitor : TsTypeSymbolVisitor
    {
        private HashSet<TsTypeSymbol> dependencies;

        private short depth = 0;

        /// <summary>
        /// Gets the dependencies required by the specified symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        /// <returns><see cref="IReadOnlyCollection{T}"/> of <see cref="TsTypeSymbol"/>.</returns>
        public IReadOnlyCollection<TsTypeSymbol> GetDependencies(TsTypeSymbol typeSymbol)
        {
            try
            {
                this.dependencies = new HashSet<TsTypeSymbol>();

                this.Visit(typeSymbol);

                return this.dependencies;
            }
            finally
            {
                this.dependencies = null;
                this.depth = 0;
            }
        }

        /// <summary>
        /// Visits the type symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        protected override void VisitTypeSymbol(TsTypeSymbol typeSymbol)
        {
            if (this.depth > 0
                && typeSymbol.IsArray
                && !typeSymbol.ElementType.IsPrimitive)
            {
                this.VisitTypeSymbol(typeSymbol.ElementType);
                return;
            }

            this.depth++;

            if (typeSymbol.IsPrimitive)
            {
                this.depth--;
                return;
            }

            if (this.depth < 2)
            {
                if (typeSymbol.Base != null)
                {
                    // A symbol will need to pass constructor parameters
                    // to the base constructor via 'super', so it will need
                    // to know all of the types for base type's properties.
                    foreach (TsPropertySymbol baseProperty in typeSymbol.Base.Properties)
                    {
                        this.VisitPropertySymbol(baseProperty);
                    }
                }

                base.VisitTypeSymbol(typeSymbol);
            }
            else
            {
                this.dependencies.Add(typeSymbol);
            }

            this.depth--;
        }
    }
}