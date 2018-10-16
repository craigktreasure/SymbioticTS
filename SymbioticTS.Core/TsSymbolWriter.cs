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
        private readonly IFileSink fileSink;

        private readonly TsSymbolImportVisitor importVisitor = new TsSymbolImportVisitor();

        /// <summary>
        /// Initializes a new instance of the <see cref="TsSymbolWriter"/> class.
        /// </summary>
        /// <param name="fileSink">The file sink.</param>
        public TsSymbolWriter(IFileSink fileSink)
        {
            this.fileSink = fileSink;
        }

        /// <summary>
        /// Writes the symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        public void WriteSymbols(IEnumerable<TsTypeSymbol> symbols)
        {
            foreach (TsTypeSymbol symbol in symbols)
            {
                this.WriteSymbolFile(symbol);
            }
        }

        private static IEnumerable<TsPropertySymbol> GetAllDistinctProperties(TsTypeSymbol symbol)
        {
            HashSet<TsPropertySymbol> propertySymbols = new HashSet<TsPropertySymbol>(
                symbol.Properties, TsPropertySymbolNameComparer.Instance);

            if (symbol.Base != null)
            {
                propertySymbols.AddRange(GetAllDistinctProperties(symbol.Base));
            }

            if (symbol.Interfaces.Count > 0)
            {
                propertySymbols.AddRange(symbol.Interfaces.SelectMany(i => GetAllDistinctProperties(i)));
            }

            return propertySymbols;
        }

        private static IEnumerable<(TsPropertySymbol property, string parameterName)> GetConstructorParameters(IEnumerable<TsPropertySymbol> propertySymbols)
        {
            return propertySymbols
                .OrderBy(p => p.IsOptional) // Sort the optional parameters to the end.
                .ThenBy(p => p.Name)
                .Select(p => (property: p, parameterName: p.Name.Camelize()));
        }

        private static string GetFileName(TsTypeSymbol symbol)
        {
            return symbol.Name + ".ts";
        }

        private static string GetPropertyTypeIdentifier(TsPropertySymbol property)
        {
            return GetTypeIdentifier(property.Type);
        }

        private static string GetTypeIdentifier(TsTypeSymbol type)
        {
            if (type.IsArray)
            {
                return $"{GetTypeIdentifier(type.ElementType)}[]";
            }
            else
            {
                return type.Name;
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

                // Write the Dto creater if necessary
                bool writeDtoMethod = symbol.IsClass && !symbol.IsAbstractClass && symbol.HasDtoInterface;
                if (writeDtoMethod)
                {
                    this.WriteClassDtoTransformMethod(writer, symbol);
                }
            }
        }

        private void WriteClassDtoTransformMethod(SourceWriter writer, TsTypeSymbol symbol)
        {
            //InterfaceTransformLookup interfaceTransformLookup = this.BuildInterfaceTransformLookup(symbol);
            DtoInterfaceTransformLookup interfaceTransformLookup = DtoInterfaceTransformLookup.BuildLookup(symbol);

            writer.WriteLine();

            TsTypeSymbol dtoInterface = symbol.DtoInterface;

            writer.WriteLine($"public static fromDto(dto: {dtoInterface.Name}): {symbol.Name}");

            using (writer.Block())
            {
                var rawClassParameters = GetConstructorParameters(GetAllDistinctProperties(symbol))
                    .Select(cp => new { cp.property, cp.parameterName, requiresTransform = cp.property.Type.RequiresDtoTransform() })
                    .Apply();

                if (rawClassParameters.Any(x => x.requiresTransform))
                {
                    foreach (var parameter in rawClassParameters.Where(x => x.requiresTransform))
                    {
                        TsDtoTypeSymbolHelper.WriteSymbolDtoTransformation(
                            propertySymbol: parameter.property,
                            valueAccessor: $"dto.{parameter.property.Name}",
                            variableName: parameter.parameterName,
                            interfaceTransformLookup: interfaceTransformLookup,
                            writer);
                    }

                    writer.WriteLine();
                }

                // Write out the class constructor.
                var classParameters = rawClassParameters.Select(p => p.requiresTransform ? p.parameterName : "dto." + p.parameterName)
                    .Apply();
                writer.Write($"return new {symbol.Name}(");
                this.WriteCommaSeparatedItems(writer, classParameters);
                writer.WriteLine(");");
            }

            // Write interface transform methods, if any.
            if (interfaceTransformLookup.Count > 0)
            {
                this.WriteClassDtoInterfaceTransfomMethods(writer, interfaceTransformLookup);
            }
        }

        private void WriteClassDtoInterfaceTransfomMethods(SourceWriter writer, DtoInterfaceTransformLookup interfaceTransformLookup)
        {
            foreach (var (dtoInterfaceSymbol, dtoInterfaceMetadata) in interfaceTransformLookup)
            {
                writer.WriteLine();

                writer.WriteLine($"private static {dtoInterfaceMetadata.TransformMethodName}(dto: {dtoInterfaceSymbol.Name}): {dtoInterfaceMetadata.ClassSymbol.Name}");

                using (writer.Block())
                {
                    var interfaceParameters = GetConstructorParameters(GetAllDistinctProperties(dtoInterfaceMetadata.ClassSymbol))
                        .Select(x => new { x.property, requiresDtoTransform = x.property.Type.RequiresDtoTransform(), x.parameterName })
                        .Apply();

                    if (interfaceParameters.Any(p => p.requiresDtoTransform))
                    {
                        foreach (var parameter in interfaceParameters.Where(p => p.requiresDtoTransform))
                        {
                            TsDtoTypeSymbolHelper.WriteSymbolDtoTransformation(
                                propertySymbol: parameter.property,
                                valueAccessor: $"dto.{parameter.property.Name}",
                                variableName: parameter.parameterName,
                                interfaceTransformLookup: interfaceTransformLookup,
                                writer);
                        }

                        writer.WriteLine();
                    }

                    IReadOnlyList<string> interfaceInitialization = interfaceParameters
                        .Select(p => $"{p.property.Name}: {(p.requiresDtoTransform ? p.parameterName : $"dto.{p.property.Name}")}")
                        .Apply();

                    writer.WriteLine("return {");
                    this.WriteCommaSeparatedItems(writer, interfaceInitialization, maxBeforeWrap: 0);
                    writer.WriteLine();
                    writer.WriteLine("};");
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
                writer.EnsureNewLine();
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
            IReadOnlyCollection<TsTypeSymbol> imports = this.importVisitor.GatherImportSymbols(symbol);

            if (imports.Count > 0)
            {
                foreach (TsTypeSymbol dependency in imports.OrderBy(t => t.Name))
                {
                    writer.WriteLine($"import {{ {dependency.Name} }} from './{dependency.Name}';");
                }

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

                    writer
                        .Write(": ")
                        .Write(GetPropertyTypeIdentifier(property))
                        .WriteLine(";");
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
            string fileName = GetFileName(symbol);

            using (Stream fileStream = this.fileSink.CreateFile(fileName))
            using (SourceWriter writer = new SourceWriter(fileStream))
            {
                this.WriteHeader(writer);
                this.WriteImports(writer, symbol);
                this.WriteObject(writer, symbol);
            }
        }
    }
}