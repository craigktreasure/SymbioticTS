using SymbioticTS.Core.IOAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SymbioticTS.Core
{
    internal class SymbioticTransformer
    {
        /// <summary>
        /// Transforms the specified input assembly.
        /// </summary>
        /// <param name="inputAssemblyPath">The input assembly path.</param>
        /// <param name="outputPath">The output path.</param>
        public void Transform(string inputAssemblyPath, string outputPath)
        {
            this.Transform(inputAssemblyPath, outputPath, out _);
        }

        /// <summary>
        /// Transforms the specified input assembly.
        /// </summary>
        /// <param name="inputAssemblyPath">The input assembly path.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="filesCreated">The files created.</param>
        public void Transform(string inputAssemblyPath, string outputPath, out IReadOnlyCollection<string> filesCreated)
        {
            if (string.IsNullOrEmpty(inputAssemblyPath) || !File.Exists(inputAssemblyPath))
            {
                throw new ArgumentException($"The specified input assembly path does not exist: '{inputAssemblyPath}'.", nameof(inputAssemblyPath));
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentException($"The specified output path is not valid: '{outputPath}'.", nameof(outputPath));
            }

            TsTypeManager typeManager = new TsTypeManager();

            Assembly assembly = Assembly.LoadFrom(inputAssemblyPath);
            IReadOnlyList<Assembly> assemblies = typeManager.DiscoverAssemblies(assembly);
            IReadOnlyList<Type> types = typeManager.DiscoverTypes(assemblies);
            IReadOnlyList<TsTypeSymbol> typeSymbols = typeManager.ResolveTypeSymbols(types);

            IFileSink fileSink = new DirectoryFileSink(outputPath);
            AuditingFileSink auditingFileSink = new AuditingFileSink(fileSink);
            TsSymbolWriter symbolWriter = new TsSymbolWriter(auditingFileSink);
            symbolWriter.WriteSymbols(typeSymbols);

            filesCreated = auditingFileSink.FilesCreated;
        }
    }
}