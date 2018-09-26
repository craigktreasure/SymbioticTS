using ReferenceProject.Shapes;
using SymbioticTS.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SymbioticTS.Core.Tests
{
    public class TsTypeSymbolTests
    {
        private class ClassWithGenericProperty
        {
            public GenericClass<int> Property { get; set; }
        }

        private class ClassWithNonArrayGenericProperty
        {
            public HashSet<int> Property { get; set; }
        }

        private class ClassWithObjectProperty
        {
            public object Property { get; set; }
        }

        [TsClass(flattenInheritance: true)]
        private class FlatTestClassWithBase : DiscoveryReferenceProject.UnannotatedClass { }

        private class GenericClass<T> { }

        [Fact]
        public void LoadAbstractClassSymbol()
        {
            Type type = typeof(DiscoveryReferenceProject.ClassWithUnannotatedBaseBase);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(
                type,
                new TsSymbolLookup(),
                TsTypeManagerOptions.Default);

            Assert.True(symbol.IsClass);
            Assert.True(symbol.IsAbstractClass);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Null(symbol.Base);
            Assert.False(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Class, symbol.Type);
        }

        [Fact]
        public void LoadAnnotatedClassSymbol()
        {
            Type type = typeof(DiscoveryReferenceProject.AnnotatedClass);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(
                type,
                new TsSymbolLookup(),
                TsTypeManagerOptions.Default);

            Assert.True(symbol.IsClass);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Null(symbol.Base);
            Assert.True(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Class, symbol.Type);
            Assert.Single(symbol.Properties);
        }

        [Fact]
        public void LoadAnnotatedInterfaceSymbol()
        {
            Type type = typeof(DiscoveryReferenceProject.IAnnotatedInterface);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(
                type,
                new TsSymbolLookup(),
                TsTypeManagerOptions.Default);

            Assert.True(symbol.IsInterface);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Null(symbol.Base);
            Assert.True(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Interface, symbol.Type);
            Assert.Single(symbol.Properties);
        }

        [Fact]
        public void LoadClassWithBaseSymbol()
        {
            TsSymbolLookup lookup = new TsSymbolLookup();
            Type type = typeof(DiscoveryReferenceProject.ClassWithUnannotatedBase);

            TsTypeSymbol baseSymbol = TsTypeSymbol.LoadFrom(type.BaseType, lookup, TsTypeManagerOptions.Default);
            lookup.Add(type.BaseType, baseSymbol);
            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, lookup, TsTypeManagerOptions.Default);

            Assert.True(symbol.IsClass);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Equal(baseSymbol, symbol.Base);
            Assert.True(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Class, symbol.Type);
            Assert.Empty(symbol.Properties);
        }

        [Fact]
        public void LoadClassWithGenericPropertySymbol()
        {
            Assert.Throws<NotSupportedException>(() => TsTypeSymbol.LoadFrom(
                typeof(ClassWithGenericProperty),
                new TsSymbolLookup(),
                TsTypeManagerOptions.Default));
        }

        [Fact]
        public void LoadClassWithNonArrayGenericPropertySymbol()
        {
            Assert.Throws<NotSupportedException>(() => TsTypeSymbol.LoadFrom(
                typeof(ClassWithNonArrayGenericProperty),
                new TsSymbolLookup(),
                TsTypeManagerOptions.Default));
        }

        [Fact]
        public void LoadClassWithObjectPropertySymbol()
        {
            TsTypeSymbol actualSymbol = TsTypeSymbol.LoadFrom(
                typeof(ClassWithObjectProperty),
                new TsSymbolLookup(),
                TsTypeManagerOptions.Default);

            Assert.Single(actualSymbol.Properties);
            Assert.Equal(TsTypeSymbol.Any, actualSymbol.Properties.Single().Type);
        }

        [Fact]
        public void LoadExplicitEnumSymbol()
        {
            Type type = typeof(Border);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, new TsSymbolLookup(), TsTypeManagerOptions.Default);

            Assert.True(symbol.IsEnum);
            Assert.Equal("ShapeBorder", symbol.Name);
            Assert.Null(symbol.Base);
            Assert.True(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Enum, symbol.Type);
            Assert.True(symbol.IsConstantEnum);

            string[] enumNames = Enum.GetNames(type);

            IReadOnlyList<TsEnumItemSymbol> enumItems = symbol.GetEnumItemSymbols().Apply();
            Assert.NotNull(enumItems);
            Assert.NotEmpty(enumItems);
            Assert.Equal(enumNames, enumItems.Select(i => i.Name));
        }

        [Fact]
        public void LoadFlatClassWithBaseSymbol()
        {
            TsSymbolLookup lookup = new TsSymbolLookup();
            Type type = typeof(FlatTestClassWithBase);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, lookup, TsTypeManagerOptions.Default);

            Assert.True(symbol.IsClass);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Null(symbol.Base);
            Assert.True(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Class, symbol.Type);
            Assert.Single(symbol.Properties);
        }

        [Fact]
        public void LoadGenericClassSymbol()
        {
            Assert.Throws<NotSupportedException>(() => TsTypeSymbol.LoadFrom(
                typeof(GenericClass<int>),
                new TsSymbolLookup(),
                TsTypeManagerOptions.Default));
        }

        [Fact]
        public void LoadImplicitEnumSymbol()
        {
            Type type = typeof(Color);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, new TsSymbolLookup(), TsTypeManagerOptions.Default);

            Assert.True(symbol.IsEnum);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Null(symbol.Base);
            Assert.False(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Enum, symbol.Type);
            Assert.False(symbol.IsConstantEnum);

            string[] enumNames = Enum.GetNames(type);

            IReadOnlyList<TsEnumItemSymbol> enumItems = symbol.GetEnumItemSymbols().Apply();
            Assert.NotNull(enumItems);
            Assert.NotEmpty(enumItems);
            Assert.Equal(enumNames, enumItems.Select(i => i.Name));
        }

        [Fact]
        public void LoadInterfaceWithUnannotatedInterfaceSymbol()
        {
            TsSymbolLookup lookup = new TsSymbolLookup();
            Type type = typeof(DiscoveryReferenceProject.InterfaceWithUnannotatedInterface);

            TsTypeSymbol expectedInterfaceSymbol =
                TsTypeSymbol.LoadFrom(typeof(DiscoveryReferenceProject.InterfaceWithUnannotatedInterfaceInterface), lookup, TsTypeManagerOptions.Default);
            lookup.Add(expectedInterfaceSymbol.TypeMetadata.Type, expectedInterfaceSymbol);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, lookup, TsTypeManagerOptions.Default);

            Assert.True(symbol.IsInterface);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Null(symbol.Base);
            TsTypeSymbol actualInterfaceSymbol = Assert.Single(symbol.Interfaces);
            Assert.Equal(expectedInterfaceSymbol, actualInterfaceSymbol);
            Assert.True(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Interface, symbol.Type);
        }

        [Fact]
        public void LoadUnannotatedClassSymbol()
        {
            Type type = typeof(DiscoveryReferenceProject.UnannotatedClass);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, new TsSymbolLookup(), TsTypeManagerOptions.Default);

            Assert.True(symbol.IsClass);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Null(symbol.Base);
            Assert.False(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Class, symbol.Type);
            Assert.Single(symbol.Properties);
        }

        [Fact]
        public void LoadUnannotatedInterfaceSymbol()
        {
            Type type = typeof(DiscoveryReferenceProject.IUnannotatedInterface);

            TsTypeSymbol symbol = TsTypeSymbol.LoadFrom(type, new TsSymbolLookup(), TsTypeManagerOptions.Default);

            Assert.True(symbol.IsInterface);
            Assert.Equal(type.Name, symbol.Name);
            Assert.Null(symbol.Base);
            Assert.False(symbol.ExplicitOptIn);
            Assert.Equal(TsSymbolType.Interface, symbol.Type);
            Assert.Single(symbol.Properties);
        }
    }
}