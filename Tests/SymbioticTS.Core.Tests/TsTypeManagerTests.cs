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

            IReadOnlyList<Assembly> actualAssemblies = manager.ResolveAssemblies(typeof(AssemblyClassToken).Assembly);

            Assert.NotNull(actualAssemblies);
            Assert.NotEmpty(actualAssemblies);
            Assert.Equal(1, actualAssemblies.Count);
        }

        [Fact]
        public void ResolveAssembliesWithReferenceAssembly()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Assembly> actualAssemblies = manager.ResolveAssemblies(
                typeof(DiscoveryReferenceProjectWithReference.AssemblyClassToken).Assembly);

            Assert.NotNull(actualAssemblies);
            Assert.NotEmpty(actualAssemblies);
            Assert.Equal(2, actualAssemblies.Count);
        }

        [Fact]
        public void ResolveSymbolsFromDiscoveryReferenceProject()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Type> discoveredTypes = manager.ResolveTypes(typeof(AssemblyClassToken).Assembly);

            IReadOnlyList<TsTypeSymbol> symbols = manager.ResolveTypeSymbols(discoveredTypes);

            Assert.Equal(discoveredTypes.Count, symbols.Count);
        }

        [Fact]
        public void ResolveSymbolsFromReferenceProject()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Type> discoveredTypes = manager.ResolveTypes(typeof(ReferenceProject.AssemblyClassToken).Assembly);

            IReadOnlyList<TsTypeSymbol> symbols = manager.ResolveTypeSymbols(discoveredTypes);

            // Check the flattened symbol.
            Assert.Contains(symbols, s => s.Name == nameof(ReferenceProject.Shapes.Triangle));
            TsTypeSymbol triangle = symbols.First(s => s.Name == nameof(ReferenceProject.Shapes.Triangle));
            Assert.True(triangle.IsClass);
            Assert.Equal(3, triangle.Properties.Count);
            Assert.Null(triangle.Base);
            Assert.Empty(triangle.Interfaces);

            Assert.NotNull(symbols.First(s => s.Name == nameof(ReferenceProject.ShapeViewModel)).DtoInterface);
            Assert.NotNull(symbols.First(s => s.Name == nameof(ReferenceProject.Shapes.IShape)).DtoInterface);
            Assert.NotNull(symbols.First(s => s.Name == nameof(ReferenceProject.Shapes.Circle)).DtoInterface);
            Assert.NotNull(symbols.First(s => s.Name == nameof(ReferenceProject.Shapes.Rectangle)).DtoInterface);

            IReadOnlyList<TsTypeSymbol> dtoInterfaces = symbols
                .Where(s => s.HasDtoInterface)
                .Select(s => s.DtoInterface).Apply();

            Assert.Equal(discoveredTypes.Count + dtoInterfaces.Count, symbols.Count);
        }

        [Fact]
        public void ResolveTypesFromReferencedAssembly()
        {
            TsTypeManager manager = new TsTypeManager();

            IEnumerable<Assembly> assemblies = manager.ResolveAssemblies(
                typeof(DiscoveryReferenceProjectWithReference.AssemblyClassToken).Assembly);
            IReadOnlyList<Type> actualTypes = manager.ResolveTypes(assemblies);

            ValidateDiscoveryReferenceProject(actualTypes);
        }

        [Fact]
        public void ResolveTypesFromReferenceProject()
        {
            TsTypeManager manager = new TsTypeManager();

            IReadOnlyList<Type> actualTypes = manager.ResolveTypes(typeof(ReferenceProject.AssemblyClassToken).Assembly);

            Assert.NotNull(actualTypes);
            Assert.NotEmpty(actualTypes);

            IReadOnlyList<Type> expectedTypes = new[]
            {
                typeof(ReferenceProject.BaseViewModel),
                typeof(ReferenceProject.ShapeViewModel),
                typeof(ReferenceProject.Shapes.BaseShape),
                typeof(ReferenceProject.Shapes.Border),
                typeof(ReferenceProject.Shapes.Circle),
                typeof(ReferenceProject.Shapes.Color),
                typeof(ReferenceProject.Shapes.IQuadrilateral),
                typeof(ReferenceProject.Shapes.IShape),
                typeof(ReferenceProject.Shapes.Rectangle),
                typeof(ReferenceProject.Shapes.Triangle),
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

            IReadOnlyList<Type> actualTypes = manager.ResolveTypes(typeof(AssemblyClassToken).Assembly);

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