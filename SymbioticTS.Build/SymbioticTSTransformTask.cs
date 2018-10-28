using Microsoft.Build.Framework;
using SymbioticTS.Core;
using System;
using System.Collections.Generic;
using System.IO;
using MSBuildTask = Microsoft.Build.Utilities.Task;

namespace SymbioticTS.Build
{
    public class SymbioticTSTransformTask : MSBuildTask
    {
        private string _assemblyReferencesPath;

        private string _inputAssemblyPath;

        private string _outputPath;

        [Required]
        public string AssemblyReferencesPath
        {
            get => this._assemblyReferencesPath;
            set
            {
                if (string.IsNullOrEmpty(value) || !File.Exists(value))
                {
                    throw new ArgumentException($"The path specified for {nameof(this.AssemblyReferencesPath)} does not exist or is not valid: '{value}'.");
                }

                this._assemblyReferencesPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the input assembly path.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        [Required]
        public string InputAssemblyPath
        {
            get => this._inputAssemblyPath;
            set
            {
                if (string.IsNullOrEmpty(value) || !File.Exists(value))
                {
                    throw new ArgumentException($"The path specified for {nameof(this.InputAssemblyPath)} does not exist or is not valid: '{value}'.");
                }

                this._inputAssemblyPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        [Required]
        public string OutputPath
        {
            get => this._outputPath;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"The path specified for {nameof(this.OutputPath)} is not valid: '{value}'.");
                }

                this._outputPath = value;
            }
        }

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(this.InputAssemblyPath) || !File.Exists(this.InputAssemblyPath))
            {
                throw new ArgumentException($"The path specified for {nameof(this.InputAssemblyPath)} does not exist or is not valid: '{this.InputAssemblyPath}'.");
            }

            if (string.IsNullOrEmpty(this.OutputPath))
            {
                throw new ArgumentException($"The path specified for {nameof(this.OutputPath)} is not valid: '{this.OutputPath}'.");
            }

            if (string.IsNullOrEmpty(this.AssemblyReferencesPath) || !File.Exists(this.AssemblyReferencesPath))
            {
                throw new ArgumentException($"The path specified for {nameof(this.AssemblyReferencesPath)} does not exist or is not valid: '{this.AssemblyReferencesPath}'.");
            }

            this.Log.LogMessage(MessageImportance.Normal, $"{nameof(SymbioticTS)}: Transforming types from {this.InputAssemblyPath} to {this.OutputPath}.");

            SymbioticTransformer transformer = new SymbioticTransformer();

            SymbioticTransformerOptions transformerOptions = new SymbioticTransformerOptions
            {
                AssemblyReferencesFilePath = this.AssemblyReferencesPath
            };

            transformer.Transform(this.InputAssemblyPath, this.OutputPath, transformerOptions, out IReadOnlyCollection<string> filesCreated);

            this.Log.LogMessage(MessageImportance.Normal, $"{nameof(SymbioticTS)}: Found and transformed {filesCreated.Count} types.");

            return true;
        }
    }
}