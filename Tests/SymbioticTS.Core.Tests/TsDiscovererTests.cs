using DiscoveryReferenceProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace SymbioticTS.Core.Tests
{
    public class TsDiscovererTests
    {
        [Fact]
        public void DiscoverAssemblies()
        {
            TsDiscoverer discoverer = new TsDiscoverer();

            IReadOnlyList<Assembly> actualAssemblies = discoverer.DiscoverSupportingAssemblies(typeof(AssemblyClassToken).Assembly);

            Assert.NotNull(actualAssemblies);
            Assert.NotEmpty(actualAssemblies);
            Assert.Equal(1, actualAssemblies.Count);
        }

        [Fact]
        public void DiscoverAssembliesWithReferenceAssembly()
        {
            TsDiscoverer discoverer = new TsDiscoverer();

            IReadOnlyList<Assembly> actualAssemblies = discoverer.DiscoverSupportingAssemblies(
                typeof(DiscoveryReferenceProjectWithReference.AssemblyClassToken).Assembly);

            Assert.NotNull(actualAssemblies);
            Assert.NotEmpty(actualAssemblies);
            Assert.Equal(2, actualAssemblies.Count);
        }

        [Fact]
        public void DiscoverTypesFromSingleAssembly()
        {
            TsDiscoverer discoverer = new TsDiscoverer();

            IReadOnlyList<Type> actualTypes = discoverer.DiscoverTypes(typeof(AssemblyClassToken).Assembly);

            ValidateDiscoveryReferenceProject(actualTypes);
        }

        [Fact]
        public void DiscoverTypesFromReferencedAssembly()
        {
            TsDiscoverer discoverer = new TsDiscoverer();

            IEnumerable<Assembly> assemblies = discoverer.DiscoverSupportingAssemblies(
                typeof(DiscoveryReferenceProjectWithReference.AssemblyClassToken).Assembly);
            IReadOnlyList<Type> actualTypes = discoverer.DiscoverTypes(assemblies);

            ValidateDiscoveryReferenceProject(actualTypes);
        }

        private static void ValidateDiscoveryReferenceProject(IReadOnlyList<Type> actualTypes)
        {
            Assert.NotNull(actualTypes);
            Assert.NotEmpty(actualTypes);

            IReadOnlyList<Type> expectedTypes = new[]
            {
                typeof(AnnotatedClass),
                typeof(AnnotatedEnum),
                typeof(ClassWithNetFrameworkProperties),
                typeof(ClassWithUnannotatedArrayClassProperty),
                typeof(ClassWithUnannotatedArrayClassProperty.UnnanotatedArrayClass),
                typeof(ClassWithUnannotatedBase),
                typeof(ClassWithUnannotatedBaseBase),
                typeof(ClassWithUnannotatedGenericBase),
                typeof(ClassWithUnannotatedGenericBaseBase<ClassWithUnannotatedGenericBaseBaseGeneric>),
                typeof(ClassWithUnannotatedGenericBaseBaseGeneric),
                typeof(ClassWithUnannotatedGenericProperties),
                typeof(ClassWithUnannotatedGenericProperties.UnnanotatedGenericClass),
                typeof(ClassWithUnannotatedGenericProperties.UnnanotatedNullableGenericStruct),
                typeof(ClassWithUnannotatedInterface),
                typeof(ClassWithUnannotatedInterfaceProperties),
                typeof(ClassWithUnannotatedInterfaceProperties.UnannotatedClass),
                typeof(ClassWithUnannotatedInterfaceProperties.UnannotatedEnum),
                typeof(ClassWithUnannotatedInterfaceProperties.IUnannotatedInterface),
                typeof(ClassWithUnannotatedInterfacePropertiesInterface),
                typeof(ClassWithUnannotatedInterfaceInterface),
                typeof(ClassWithUnannotatedNestedProperties),
                typeof(ClassWithUnannotatedNestedProperties.IUnannotatedPropertyInterface),
                typeof(ClassWithUnannotatedNestedProperties.UnannotatedClass),
                typeof(ClassWithUnannotatedNestedProperties.UnannotatedPropertyClass),
                typeof(ClassWithUnannotatedNestedProperties.UnannotatedPropertyEnum),
                typeof(ClassWithUnannotatedProperties),
                typeof(ClassWithUnannotatedProperties.IUnannotatedInterface),
                typeof(ClassWithUnannotatedProperties.UnannotatedClass),
                typeof(ClassWithUnannotatedProperties.UnannotatedEnum),
                typeof(IAnnotatedInterface),
                typeof(InterfaceWithUnannotatedInterface),
                typeof(InterfaceWithUnannotatedInterfaceInterface),
            }.OrderBy(t => t.Name).Apply();

            Assert.Equal(expectedTypes, actualTypes.OrderBy(t => t.Name));
            Assert.Equal(expectedTypes.Count, actualTypes.Count);
        }
    }
}