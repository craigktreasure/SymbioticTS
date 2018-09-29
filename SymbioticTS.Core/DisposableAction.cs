using System;

namespace SymbioticTS.Core
{
    /// <summary>
    /// Class DisposableAction.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    internal class DisposableAction : IDisposable
    {
        /// <summary>
        /// The dispose action.
        /// </summary>
        private readonly Action disposeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableAction"/> class.
        /// </summary>
        /// <param name="disposeAction">The dispose action.</param>
        public DisposableAction(Action disposeAction)
        {
            this.disposeAction = disposeAction;
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
                    this.disposeAction?.Invoke();
                }

                this.disposedValue = true;
            }
        }

        #endregion IDisposable Support
    }
}