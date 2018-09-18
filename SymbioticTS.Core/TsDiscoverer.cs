using SymbioticTS.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class TsDiscoverer
    {
        /// <summary>
        /// Discovers the assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>IReadOnlyList&lt;Assembly&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IReadOnlyList<Assembly> DiscoverSupportingAssemblies(Assembly assembly)
        {
            return DiscoverAllSupportingAssemblies(assembly).Distinct().Apply();
        }

        /// <summary>
        /// Discovers the TypeScript types in the assemblies.
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
        /// Discovers the TypeScript types in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of <see cref="Type"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IReadOnlyList<Type> DiscoverTypes(Assembly assembly)
        {
            IEnumerable<Type> typeScriptTypes = assembly.DefinedTypes
                .Where(IsTypeScriptType)
                .SelectMany(DiscoverSupportingTypes)
                .Distinct();

            return typeScriptTypes.Apply();
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

        private static IEnumerable<Type> DiscoverSupportingTypes(Type type)
        {
            if (type == null)
            {
                yield break;
            }

            if (type.IsArray)
            {
                foreach (Type arraySupportingType in DiscoverSupportingTypes(type.GetElementType()))
                {
                    yield return arraySupportingType;
                }

                yield break;
            }

            if (type.IsGenericType)
            {
                foreach (Type genericSupportingType in type.GetGenericArguments().SelectMany(DiscoverSupportingTypes))
                {
                    yield return genericSupportingType;
                }
            }

            if (type.IsNetFramework())
            {
                yield break;
            }

            foreach (Type baseSupportingType in DiscoverSupportingTypes(type.BaseType))
            {
                yield return baseSupportingType;
            }

            foreach (Type interfaceSupportingType in type.GetInterfaces().SelectMany(DiscoverSupportingTypes))
            {
                yield return interfaceSupportingType;
            }

            foreach (Type propertySupportingType in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).SelectMany(p => DiscoverSupportingTypes(p.PropertyType)))
            {
                yield return propertySupportingType;
            }

            yield return type;
        }

        private static bool HasSymbioticTSAbstractionsReference(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies().Any(name => name.Name == "SymbioticTS.Abstractions");
        }

        private static bool IsTypeScriptType(Type type)
        {
            return Attribute.IsDefined(type, typeof(TsClassAttribute))
                || Attribute.IsDefined(type, typeof(TsEnumAttribute))
                || Attribute.IsDefined(type, typeof(TsInterfaceAttribute))
                || Attribute.IsDefined(type, typeof(TsDtoAttribute));
        }
    }
}