using System;

namespace ManagedDismapi {
    /// <summary>
    /// Represents a open Windows image.
    /// </summary>
    public sealed class WindowsImage : IDisposable {
        private readonly uint session;
        private bool disposed;

        internal WindowsImage(uint session) {
            this.session = session;
        }

        /// <summary>
        /// Closes the image's associated session. You will be unable to perform operations on this image after disposing it.
        /// </summary>
        public void Dispose() {
            if(disposed) {
                return;
            }

            try {
                NativeMethods.DismCloseSession(session);
            } catch(Exception e) {
                Utils.HandleHResult(e);
            }

            disposed = true;

            GC.SuppressFinalize(this);
        }

        private void EnsureNotDisposed() {
            if(disposed) {
                throw new ObjectDisposedException(GetType().FullName, "This image's session has already been closed.");
            }
        }

        ~WindowsImage() {
            try {
                NativeMethods.DismCloseSession(session);
            } catch(Exception e) {
                Utils.HandleHResult(e);
            }
        }
    }
}
