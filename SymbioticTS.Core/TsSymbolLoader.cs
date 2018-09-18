using System;
using System.Collections.Generic;
using System.Linq;

namespace SymbioticTS.Core
{
    internal static class TsSymbolLoader
    {
        private enum NodeState
        {
            Default,
            Visited,
            Visiting
        }

        /// <summary>
        /// Loads type symbols for the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>A <see cref="IReadOnlyList{T}"/> of <see cref="TsTypeSymbol"/>.</returns>
        public static IReadOnlyList<TsTypeSymbol> Load(IEnumerable<Type> types)
        {
            List<TsTypeSymbol> results = new List<TsTypeSymbol>();
            TsSymbolLookup symbolLookup = new TsSymbolLookup();

            foreach (Type type in SortTopologically(types))
            {
                TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, symbolLookup);

                symbolLookup.Add(type, symbol);
                results.Add(symbol);
            }

            return results;
        }

        private static IEnumerable<Type> GetDependencies(Type node)
        {
            if (node == null || node.IsNetFramework())
            {
                yield break;
            }

            {
                foreach (Type dependencyNode in GetDependencies(node.BaseType))
                {
                    yield return dependencyNode;
                }
            }

            {
                foreach (Type dependencyNode in node.GetInterfaces()
                    .SelectMany(GetDependencies))
                {
                    yield return dependencyNode;
                }
            }

            {
                foreach (Type dependencyNode in node.GetProperties()
                    .SelectMany(p => GetDependencies(p.PropertyType)))
                {
                    yield return dependencyNode;
                }
            }
        }

        private static IReadOnlyList<Type> SortTopologically(IEnumerable<Type> types)
        {
            IReadOnlyList<Type> allTypes = types.Apply();

            Dictionary<Type, NodeState> state = allTypes.ToDictionary(m => m, _ => NodeState.Default);
            List<Type> sortedList = new List<Type>();

            foreach (Type type in allTypes)
            {
                Visit(type, sortedList, state);
            }

            return sortedList;
        }

        private static void Visit(Type node, IList<Type> sortedList, IDictionary<Type, NodeState> state)
        {
            switch (state[node])
            {
                case NodeState.Visited:
                    // Already done.
                    return;

                case NodeState.Visiting:
                    throw new InvalidOperationException($"Circular dependencies are not currently supported: {node.FullName}.");
            }

            state[node] = NodeState.Visiting;

            foreach (Type dependencyNode in GetDependencies(node).Apply())
            {
                Visit(dependencyNode, sortedList, state);
            }

            state[node] = NodeState.Visited;

            sortedList.Add(node);
        }
    }
}