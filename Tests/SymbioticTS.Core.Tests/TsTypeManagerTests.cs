using DiscoveryReferenceProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace SymbioticTS.Core.Tests
{
    public class TsTypeManagerTests
    {
        [Fact]
        public void ResolveAssemblies()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Assembly> actualAssemblies = manager.DiscoverAssemblies(typeof(AssemblyClassToken).Assembly);

            Assert.NotNull(actualAssemblies);
            Assert.NotEmpty(actualAssemblies);
            Assert.Equal(1, actualAssemblies.Count);
        }

        [Fact]
        public void ResolveAssembliesWithReferenceAssembly()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Assembly> actualAssemblies = manager.DiscoverAssemblies(
                typeof(DiscoveryReferenceProjectWithReference.AssemblyClassToken).Assembly);

            Assert.NotNull(actualAssemblies);
            Assert.NotEmpty(actualAssemblies);
            Assert.Equal(2, actualAssemblies.Count);
        }

        [Fact]
        public void ResolveSymbolsFromDiscoveryReferenceProject()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Type> discoveredTypes = manager.DiscoverTypes(typeof(AssemblyClassToken).Assembly);

            IReadOnlyList<TsTypeSymbol> symbols = manager.ResolveTypeSymbols(discoveredTypes);

            Assert.Equal(discoveredTypes.Count, symbols.Count);
        }

        [Fact]
        public void ResolveSymbolsFromReferenceProject()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Type> discoveredTypes = manager.DiscoverTypes(typeof(ScenarioReferenceProject.AssemblyClassToken).Assembly);

            IReadOnlyList<TsTypeSymbol> symbols = manager.ResolveTypeSymbols(discoveredTypes);

            // Check the flattened symbol.
            Assert.Contains(symbols, s => s.Name == nameof(ScenarioReferenceProject.Shapes.Triangle));
            TsTypeSymbol triangle = symbols.First(s => s.Name == nameof(ScenarioReferenceProject.Shapes.Triangle));
            Assert.True(triangle.IsClass);
            Assert.Equal(3, triangle.Properties.Count);
            Assert.Null(triangle.Base);
            Assert.Empty(triangle.Interfaces);

            Assert.NotNull(symbols.First(s => s.Name == nameof(ScenarioReferenceProject.ShapeViewModel)).DtoInterface);
            Assert.NotNull(symbols.First(s => s.Name == nameof(ScenarioReferenceProject.Shapes.IShape)).DtoInterface);
            Assert.NotNull(symbols.First(s => s.Name == nameof(ScenarioReferenceProject.Shapes.Circle)).DtoInterface);
            Assert.NotNull(symbols.First(s => s.Name == nameof(ScenarioReferenceProject.Shapes.Rectangle)).DtoInterface);

            IReadOnlyList<TsTypeSymbol> dtoInterfaces = symbols
                .Where(s => s.HasDtoInterface)
                .Select(s => s.DtoInterface).Apply();

            Assert.Equal(discoveredTypes.Count + dtoInterfaces.Count, symbols.Count);
        }

        [Fact]
        public void ResolveTypesFromReferencedAssembly()
        {
            TsTypeManager manager = new TsTypeManager();

            IEnumerable<Assembly> assemblies = manager.DiscoverAssemblies(
                typeof(DiscoveryReferenceProjectWithReference.AssemblyClassToken).Assembly);
            IReadOnlyList<Type> actualTypes = manager.DiscoverTypes(assemblies);

            ValidateDiscoveryReferenceProject(actualTypes);
        }

        [Fact]
        public void ResolveTypesFromReferenceProject()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Type> actualTypes = manager.DiscoverTypes(typeof(ScenarioReferenceProject.AssemblyClassToken).Assembly);

            Assert.NotNull(actualTypes);
            Assert.NotEmpty(actualTypes);

            IReadOnlyList<Type> expectedTypes = new[]
            {
                typeof(ScenarioReferenceProject.BaseViewModel),
                typeof(ScenarioReferenceProject.ShapeViewModel),
                typeof(ScenarioReferenceProject.Shapes.BaseShape),
                typeof(ScenarioReferenceProject.Shapes.Border),
                typeof(ScenarioReferenceProject.Shapes.Circle),
                typeof(ScenarioReferenceProject.Shapes.Color),
                typeof(ScenarioReferenceProject.Shapes.IQuadrilateral),
                typeof(ScenarioReferenceProject.Shapes.IShape),
                typeof(ScenarioReferenceProject.Shapes.Rectangle),
                typeof(ScenarioReferenceProject.Shapes.Triangle),
            }.OrderBy(t => t.Name).Apply();

            if (expectedTypes.Count > actualTypes.Count)
            {
                Assert.Empty(expectedTypes.Except(actualTypes));
            }

            if (expectedTypes.Count < actualTypes.Count)
            {
                Assert.Empty(actualTypes.Except(expectedTypes));
            }

            Assert.Equal(expectedTypes.Count, actualTypes.Count);
            Assert.Equal(expectedTypes, actualTypes.OrderBy(t => t.Name));
        }

        [Fact]
        public void ResolveTypesFromSingleAssembly()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Type> actualTypes = manager.DiscoverTypes(typeof(AssemblyClassToken).Assembly);

            ValidateDiscoveryReferenceProject(actualTypes);
        }

        [Fact]
        public void ResolveTypeSymbolsWithPotentialDtoNameCollision()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Type> discoveredTypes = new[]
            {
                typeof(UnitTestReferenceLibrary.WithOverlap.Circle),
                typeof(UnitTestReferenceLibrary.WithOverlap.ICircle),
            };

            IReadOnlyList<TsTypeSymbol> symbols = manager.ResolveTypeSymbols(discoveredTypes);

            Assert.Equal(4, symbols.Count);

            Assert.Equal(new[]
            {
                "Circle",
                "ICircle",
                "ICircleDto",
                "ICircleDto1",
            }, symbols.Select(x => x.Name).OrderBy(x => x));
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
                //typeof(ClassWithUnannotatedGenericBase),
                //typeof(ClassWithUnannotatedGenericBaseBase<ClassWithUnannotatedGenericBaseBaseGeneric>),
                //typeof(ClassWithUnannotatedGenericBaseBaseGeneric),
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

            if (expectedTypes.Count > actualTypes.Count)
            {
                Assert.Empty(expectedTypes.Except(actualTypes));
            }

            if (expectedTypes.Count < actualTypes.Count)
            {
                Assert.Empty(actualTypes.Except(expectedTypes));
            }

            Assert.Equal(expectedTypes.Count, actualTypes.Count);
            Assert.Equal(expectedTypes, actualTypes.OrderBy(t => t.Name));
        }
    }
}