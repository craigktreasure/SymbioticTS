using Humanizer;
using SymbioticTS.Core.IOAbstractions;
using SymbioticTS.Core.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymbioticTS.Core
{
    internal class TsSymbolWriter
    {
        private readonly TsDirectDependencyTypeSymbolVisitor dependencyVisitor = new TsDirectDependencyTypeSymbolVisitor();

        private readonly IFileSink fileSink;

        public TsSymbolWriter(IFileSink fileSink)
        {
            this.fileSink = fileSink;
        }

        public void WriteSymbols(IEnumerable<TsTypeSymbol> symbols)
        {
            foreach (TsTypeSymbol symbol in symbols)
            {
                this.WriteSymbolFile(symbol);
            }
        }

        private static string GetFileName(TsTypeSymbol symbol)
        {
            return symbol.Name + ".ts";
        }

        private static IEnumerable<(TsPropertySymbol property, string parameterName)> GetConstructorParameters(IEnumerable<TsPropertySymbol> propertySymbols)
        {
            return propertySymbols
                .OrderBy(p => p.IsOptional) // Sort the optional parameters to the end.
                .ThenBy(p => p.Name)
                .Select(p => (property: p, parameterName: p.Name.Camelize()));
        }

        private static string GetPropertyTypeIdentifier(TsPropertySymbol property)
        {
            if (property.Type.IsArray)
            {
                return $"{property.Type.ElementType.Name}[]";
            }
            else
            {
                return property.Type.Name;
            }
        }

        private void WriteClass(SourceWriter writer, TsTypeSymbol symbol)
        {
            writer.Write("export ");

            if (symbol.IsAbstractClass)
            {
                writer.Write("abstract ");
            }

            writer.Write("class ").Write(symbol.Name);

            if (symbol.Base != null)
            {
                writer.Write(" extends ").Write(symbol.Base.Name);
            }

            if (symbol.Interfaces?.Any() ?? false)
            {
                writer
                    .Write(" implements ")
                    .Write(string.Join(", ", symbol.Interfaces.Select(i => i.Name)));
            }

            writer.WriteLine();

            using (writer.Block())
            {
                // Write the properties.
                foreach (TsPropertySymbol property in symbol.Properties)
                {
                    writer.Write("public ");

                    if (property.IsReadOnly)
                    {
                        writer.Write("readonly ");
                    }

                    writer.Write(property.Name);

                    if (property.IsOptional)
                    {
                        writer.Write("?");
                    }

                    writer.Write(": ").Write(GetPropertyTypeIdentifier(property));

                    writer.WriteLine(";");
                }

                if (symbol.Properties.Count > 0)
                {
                    writer.WriteLine();
                }

                // Write the constructor.
                writer.Write("constructor(");

                var baseProperties = symbol.Base?.Properties ?? Enumerable.Empty<TsPropertySymbol>();
                var basePropertyInfos = GetConstructorParameters(baseProperties).Apply();
                var propertyInfos = GetConstructorParameters(symbol.Properties).Apply();
                var parameterInfos = GetConstructorParameters(baseProperties.Concat(symbol.Properties)).Apply();

                IReadOnlyList<string> parameters = parameterInfos
                    .Select(p => $"{p.parameterName}{(p.property.IsOptional ? "?" : string.Empty)}: {GetPropertyTypeIdentifier(p.property)}")
                    .Apply();

                this.WriteCommaSeparatedItems(writer, parameters);

                writer.WriteLine(")");
                using (writer.Block())
                {
                    if (symbol.Base != null)
                    {
                        // Make call to super(...).
                        writer.Write("super(");
                        this.WriteCommaSeparatedItems(writer, basePropertyInfos.Select(p => p.parameterName).Apply());
                        writer.WriteLine(");");

                        if (propertyInfos.Any())
                        {
                            writer.WriteLine();
                        }
                    }

                    foreach (var (property, parameterName) in propertyInfos)
                    {
                        writer.WriteLine($"this.{property.Name} = {parameterName};");
                    }
                }
            }
        }

        private void WriteCommaSeparatedItems(SourceWriter writer, IReadOnlyList<string> items, int maxBeforeWrap = 3)
        {
            if (items.Count <= maxBeforeWrap)
            {
                writer.Write(string.Join(", ", items));
            }
            else
            {
                writer.WriteLine();
                using (writer.Indent())
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        writer.Write(items[i]);

                        if (i != items.Count - 1)
                        {
                            writer.WriteLine(",");
                        }
                    }
                }
            }
        }

        private void WriteEnumeration(SourceWriter writer, TsTypeSymbol symbol)
        {
            writer.WriteLine($"export enum {symbol.Name}");

            using (writer.Block())
            {
                foreach (TsEnumItemSymbol enumItem in symbol.GetEnumItemSymbols())
                {
                    writer.WriteLine($"{enumItem.Name} = {enumItem.Value},");
                }
            }
        }

        private void WriteHeader(SourceWriter writer)
        {
            writer.WriteLine(@"/**
 * This file was generated by SymbioticTS.
 * All changes will be lost the next time the file is generated.
 */");
            writer.WriteLine();
        }

        private void WriteImports(SourceWriter writer, TsTypeSymbol symbol)
        {
            IReadOnlyList<TsTypeSymbol> dependencies = this.dependencyVisitor
                .GetDependencies(symbol).OrderBy(t => t.Name).Apply();

            foreach (TsTypeSymbol dependency in dependencies)
            {
                writer.WriteLine($"import {{ {dependency.Name} }} from './{dependency.Name}';");
            }

            if (dependencies.Count > 0)
            {
                writer.WriteLine();
            }
        }

        private void WriteInterface(SourceWriter writer, TsTypeSymbol symbol)
        {
            writer.Write("export interface ").Write(symbol.Name);

            if (symbol.Interfaces?.Any() ?? false)
            {
                writer
                    .Write(" extends ")
                    .Write(string.Join(", ", symbol.Interfaces.Select(i => i.Name)));
            }

            writer.WriteLine();

            using (writer.Block())
            {
                foreach (TsPropertySymbol property in symbol.Properties)
                {
                    if (symbol.IsClass)
                    {
                        writer.Write("public ");
                    }

                    if (property.IsReadOnly)
                    {
                        writer.Write("readonly ");
                    }

                    writer.Write(property.Name);

                    if (property.IsOptional)
                    {
                        writer.Write("?");
                    }

                    writer.Write(": ");

                    if (property.Type.IsArray)
                    {
                        writer.Write(property.Type.ElementType.Name).Write("[]");
                    }
                    else
                    {
                        writer.Write(property.Type.Name);
                    }

                    writer.WriteLine(";");
                }
            }
        }

        private void WriteObject(SourceWriter writer, TsTypeSymbol symbol)
        {
            switch (symbol.Kind)
            {
                case TsSymbolKind.Class:
                    this.WriteClass(writer, symbol);
                    break;

                case TsSymbolKind.Enum:
                    this.WriteEnumeration(writer, symbol);
                    break;

                case TsSymbolKind.Interface:
                    this.WriteInterface(writer, symbol);
                    break;

                default:
                    throw new NotSupportedException($"The symbol type ({symbol.Kind}) is not supported at the top file level.");
            }
        }

        private void WriteSymbolFile(TsTypeSymbol symbol)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (SourceWriter writer = new SourceWriter(memoryStream))
            using (StreamReader reader = new StreamReader(memoryStream, SourceWriter.DefaultEncoding))
            {
                string fileName = GetFileName(symbol);

                this.WriteHeader(writer);
                this.WriteImports(writer, symbol);
                this.WriteObject(writer, symbol);

                writer.Flush();

                memoryStream.Seek(0, SeekOrigin.Begin);

                this.fileSink.CreateFile(fileName, reader.ReadToEnd());
            }
        }
    }
}