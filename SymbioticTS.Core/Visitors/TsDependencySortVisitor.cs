using System;
using System.Collections.Generic;
using System.Linq;

namespace SymbioticTS.Core.Visitors
{
    internal class TsDependencySortVisitor : TsTypeVisitor
    {
        private Dictionary<Type, bool> typesVisited;

        private List<Type> orderedTypes;

        /// <summary>
        /// Sorts the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> of <see cref="Type"/>.</returns>
        public IReadOnlyList<Type> Sort(IEnumerable<Type> types)
        {
            IReadOnlyList<Type> allTypes = types.Apply();

            this.typesVisited = allTypes.ToDictionary(t => t, _ => false);
            this.orderedTypes = new List<Type>();

            foreach (Type type in allTypes)
            {
                this.Visit(type);
            }

            this.typesVisited = null;

            return this.orderedTypes;
        }

        /// <summary>
        /// Visits the type.
        /// </summary>
        /// <param name="type">The type.</param>
        protected override void VisitType(Type type)
        {
            base.VisitType(type);

            if (this.typesVisited.TryGetValue(type, out bool visited) && !visited)
            {
                this.typesVisited[type] = true;
                this.orderedTypes.Add(type);
            }
        }
    }
}