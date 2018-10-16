using System;
using System.IO;
using System.Text;

namespace SymbioticTS.Core
{
    /// <summary>
    /// Class SourceWriter.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    internal class SourceWriter : IDisposable
    {
        /// <summary>
        /// The default encoding to be used.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// The indentation value.
        /// </summary>
        private const string indentationValue = "    ";

        /// <summary>
        /// The writer.
        /// </summary>
        private readonly StreamWriter writer;

        /// <summary>
        /// At beginning.
        /// </summary>
        private bool atBeginning = true;

        /// <summary>
        /// The indentation level.
        /// </summary>
        private int indentationLevel = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public SourceWriter(Stream stream)
            : this(stream, DefaultEncoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceWriter" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        public SourceWriter(Stream stream, Encoding encoding)
        {
            this.writer = new StreamWriter(stream, encoding);
        }

        /// <summary>
        /// Write within an indented block for the lifetime of the <see cref="IDisposable"/>.
        /// </summary>
        /// <returns><see cref="IDisposable"/>.</returns>
        public IDisposable Block()
        {
            this.WriteLine("{");

            this.IncreaseIndentation();

            return new DisposableAction(() =>
            {
                this.DecreaseIndentation();
                this.WriteLine("}");
            });
        }

        /// <summary>
        /// Decreases the indentation.
        /// </summary>
        /// <returns>SourceWriter.</returns>
        public SourceWriter DecreaseIndentation()
        {
            if (this.indentationLevel > 0)
            {
                this.indentationLevel--;
            }

            return this;
        }

        /// <summary>
        /// Ensures the writer is on a new line.
        /// </summary>
        /// <returns>SourceWriter.</returns>
        public SourceWriter EnsureNewLine()
        {
            if (this.atBeginning)
            {
                return this;
            }

            return this.WriteLine();
        }

        /// <summary>
        /// Flushes all buffers to the underlying stream.
        /// </summary>
        public void Flush()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// Increases the indentation.
        /// </summary>
        /// <returns>SourceWriter.</returns>
        public SourceWriter IncreaseIndentation()
        {
            this.indentationLevel++;

            return this;
        }

        /// <summary>
        /// Indents for the lifetime of the <see cref="IDisposable"/>.
        /// </summary>
        /// <returns><see cref="IDisposable"/>.</returns>
        public IDisposable Indent()
        {
            this.IncreaseIndentation();

            return new DisposableAction(() => this.DecreaseIndentation());
        }

        /// <summary>
        /// Outdents for the lifetime of the <see cref="IDisposable"/>.
        /// </summary>
        /// <returns><see cref="IDisposable"/>.</returns>
        public IDisposable Outdent()
        {
            if (this.indentationLevel > 0)
            {
                this.DecreaseIndentation();

                return new DisposableAction(() => this.IncreaseIndentation());
            }
            else
            {
                return new DisposableAction(null);
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>SourceWriter.</returns>
        public SourceWriter Write(string value)
        {
            if (this.atBeginning)
            {
                this.WriteIndentation();
            }

            this.writer.Write(value);

            this.atBeginning = false;

            return this;
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <returns>SourceWriter.</returns>
        public SourceWriter WriteLine()
        {
            this.writer.WriteLine();

            this.atBeginning = true;

            return this;
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>SourceWriter.</returns>
        public SourceWriter WriteLine(string value)
        {
            if (this.atBeginning)
            {
                this.WriteIndentation();
            }

            this.writer.WriteLine(value);

            this.atBeginning = true;

            return this;
        }

        /// <summary>
        /// Writes the indentation.
        /// </summary>
        private void WriteIndentation()
        {
            if (this.indentationLevel > 0)
            {
                for (int i = 0; i < this.indentationLevel; i++)
                {
                    this.writer.Write(indentationValue);
                }

                this.atBeginning = false;
            }
        }

        #region IDisposable Support

        /// <summary>
        /// The disposed value.
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.writer.Dispose();
                }

                this.disposedValue = true;
            }
        }

        #endregion IDisposable Support
    }
}