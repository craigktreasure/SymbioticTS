using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SymbioticTS.Core.Visitors
{
    internal class TsFullDependencyVisitor : TsTypeVisitor
    {
        private Dictionary<Type, List<Type>> typeDependencies;

        private Type currentType;
        private List<Type> currentTypeDependencies;

        /// <summary>
        /// Gets all of the types associated with the specified list of types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> of <see cref="Type"/>.</returns>
        public IReadOnlyList<Type> GetAllTypes(IEnumerable<Type> types)
        {
            IReadOnlyList<Type> allTypes = types.Distinct().Apply();

            try
            {
                this.typeDependencies = allTypes.ToDictionary(t => t, _ => new List<Type>());

                foreach (Type type in types)
                {
                    if (this.currentType == null || type != this.currentType)
                    {
                        this.currentType = type;
                        this.currentTypeDependencies = this.typeDependencies[type];
                    }

                    this.Visit(type);
                }

                allTypes = this.typeDependencies.Keys.Concat(this.typeDependencies.Values.SelectMany(t => t)).Distinct().Apply();
            }
            finally
            {
                this.currentType = null;
                this.typeDependencies = null;
            }

            return allTypes;
        }

        /// <summary>
        /// Visits the type.
        /// </summary>
        /// <param name="type">The type.</param>
        protected override void VisitType(Type type)
        {
            if (type != this.currentType && !type.IsNetFramework())
            {
                this.currentTypeDependencies.Add(type);
            }

            if (this.typeDependencies.ContainsKey(type))
            {
                TsTypeMetadata typeMetadata = TsTypeMetadata.LoadFrom(type);

                if (typeMetadata.Flatten)
                {
                    foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        this.VisitProperty(propertyInfo);
                    }

                    return;
                }
            }

            base.VisitType(type);
        }
    }
}