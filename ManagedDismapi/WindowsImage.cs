using System;
using System.Threading;
using ManagedDismapi.Interop;

namespace ManagedDismapi {
    /// <summary>
    /// Represents an open Windows image.
    /// </summary>
    public sealed class WindowsImage : IDisposable {
        private readonly uint session;
        private bool disposed;

        internal WindowsImage(uint session) {
            this.session = session;
        }

        /// <summary>
        /// Commits changes made to the Windows image without closing it/
        /// </summary>
        /// <param name="options">Options to for committing the image.</param>
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the command.</param>
        public void Commit(CommitOptions options, CancellationToken cancellationToken, IProgress<DismProgressInfo> progress) {
            EnsureNotDisposed();

            try {
                NativeMethods.DismCommitImage(
                    session,
                    options,
                    cancellationToken.WaitHandle.GetSafeWaitHandle(),
                    Utils.MakeNativeCallback(progress),
                    IntPtr.Zero
                );
            } catch(Exception e) {
                Utils.HandleHResult(e);
            }
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
