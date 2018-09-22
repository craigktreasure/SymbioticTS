using System;
using System.Collections.Generic;
using System.Linq;

namespace SymbioticTS.Core.Visitors
{
    internal class TsDependencySortVisitor : TsTypeVisitor
    {
        private Dictionary<Type, bool> typesVisited;

        private List<Type> orderedTypes;

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