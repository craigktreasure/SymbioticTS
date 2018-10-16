namespace SymbioticTS.Core.Visitors
{
    internal abstract class TsTypeSymbolVisitor
    {
        /// <summary>
        /// Visits the specified type symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        public void Visit(TsTypeSymbol typeSymbol)
        {
            this.VisitTypeSymbol(typeSymbol);
        }

        /// <summary>
        /// Visits the base type symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        protected virtual void VisitBaseTypeSymbol(TsTypeSymbol typeSymbol)
        {
            this.VisitTypeSymbol(typeSymbol);
        }

        /// <summary>
        /// Visits the data transfer interface type symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        protected virtual void VisitDtoInterfaceTypeSymbol(TsTypeSymbol typeSymbol)
        {
            this.VisitTypeSymbol(typeSymbol);
        }

        /// <summary>
        /// Visits the interface type symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        protected virtual void VisitInterfaceTypeSymbol(TsTypeSymbol typeSymbol)
        {
            this.VisitTypeSymbol(typeSymbol);
        }

        /// <summary>
        /// Visits the property symbol.
        /// </summary>
        /// <param name="propertySymbol">The property symbol.</param>
        protected virtual void VisitPropertySymbol(TsPropertySymbol propertySymbol)
        {
            this.VisitPropertyTypeSymbol(propertySymbol.Type);
        }

        /// <summary>
        /// Visits the property type symbol.
        /// </summary>
        /// <param name="propertyTypeSymbol">The property type symbol.</param>
        protected virtual void VisitPropertyTypeSymbol(TsTypeSymbol propertyTypeSymbol)
        {
            this.VisitTypeSymbol(propertyTypeSymbol);
        }

        /// <summary>
        /// Visits the type symbol.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        protected virtual void VisitTypeSymbol(TsTypeSymbol typeSymbol)
        {
            if (typeSymbol.IsArray)
            {
                this.VisitTypeSymbol(typeSymbol.ElementType);
            }

            if (typeSymbol.IsPrimitive)
            {
                return;
            }

            if (typeSymbol.HasDtoInterface)
            {
                this.VisitDtoInterfaceTypeSymbol(typeSymbol.DtoInterface);
            }

            if (typeSymbol.Base != null)
            {
                this.VisitBaseTypeSymbol(typeSymbol.Base);
            }

            foreach (TsTypeSymbol interfaceType in typeSymbol.Interfaces)
            {
                this.VisitInterfaceTypeSymbol(interfaceType);
            }

            foreach (TsPropertySymbol property in typeSymbol.Properties)
            {
                this.VisitPropertySymbol(property);
            }
        }
    }
}