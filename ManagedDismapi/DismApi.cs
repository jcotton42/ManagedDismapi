using System;
using System.Runtime.ExceptionServices;
using System.Security;

namespace ManagedDismapi {
    /// <summary>
    /// TODO
    /// </summary>
    public static class DismApi {
        /// <summary>
        /// Initializes the DISM API for this process.
        /// </summary>
        /// <param name="logLevel">The logging level.</param>
        /// <param name="logPath">Relative or absolute path to a log file. Defaults to %windir%\Logs\DISM\dism.log if <c>null</c> is passed.</param>
        /// <param name="scratchPath">Relative or absolute path to a scratch directory. Defaults to %windir%\Temp if <c>null</c> is passed.</param>
        /// <exception cref="InvalidOperationException">This method has already been called without a matching <see cref="Shutdown"/> call.</exception>
        /// <exception cref="SecurityException">The calling process does not have administrative privileges.</exception>
        /// <remarks>The calling process must have administrative privileges.</remarks>
        /// <seealso cref="Shutdown"/>
        public static void Initialize(LogLevel logLevel, string logPath, string scratchPath) {
            try {
                NativeMethods.DismInitialize(logLevel, logPath, scratchPath);
            } catch(Exception e) {
                HandleHResult(e);
            }
        }

        /// <summary>
        /// Shuts down the DISM API for this process.
        /// </summary>
        /// <exception cref="InvalidOperationException">DISM sessions are still open.</exception>
        /// <seealso cref="Initialize"/>
        public static void Shutdown() {
            try {
                NativeMethods.DismShutdown();
            } catch(Exception e) {
                HandleHResult(e);
            }
        }

        /// <summary>Opens the running OS image for servicing.</summary>
        /// <inheritdoc cref="OpenImage(string, string, string)"/>
        public static WindowsImage OpenOSImage() => OpenImage(NativeMethods.DISM_ONLINE_IMAGE, null, null);

        /// <inheritdoc cref="OpenImage(string, string, string)"/>
        public static WindowsImage OpenImage(string imagePath) => OpenImage(imagePath, null, null);

        /// <summary>
        /// Opens an offline Windows image for servicing.
        /// </summary>
        /// <param name="imagePath">An absolute or relative path to the directory of an offline Windows image.</param>
        /// <param name="windowsDirectory">The path to the Windows folder relative to <paramref name="imagePath"/>. Defaults to "Windows" if <c>null</c> is passed.</param>
        /// <param name="systemDrive">The letter of the system drive that contains the boot manager. Defaults to the drive containing <paramref name="imagePath"/> if <c>null</c> is passed.</param>
        /// <returns>A new <see cref="WindowsImage"/> corresponding to the offline image.</returns>
        /// <exception cref="TODO">TODO</exception>
        public static WindowsImage OpenImage(string imagePath, string windowsDirectory, string systemDrive) {
            uint session = 0;

            try {
                NativeMethods.DismOpenSession(imagePath, windowsDirectory, systemDrive, out session);
            } catch(Exception e) {
                HandleHResult(e);
            }

            return new WindowsImage(session);
        }

        internal static void HandleHResult(Exception e) {
            NativeMethods.DismGetLastErrorMessage(out IntPtr ptr);
            var message = DismString.FromIntPtr(ptr).Value;

            switch(e.HResult) {
                //TODO: case NativeMethods.DISMAPI_E_DISMAPI_ALREADY_INITIALIZED (need to find the value of this, not defined in the header)
                case NativeMethods.ERROR_ELEVATION_REQUIRED_HR:
                    throw new SecurityException(message, e);
                case NativeMethods.DISMAPI_E_OPEN_SESSION_HANDLES:
                    throw new InvalidOperationException(message, e);
                default:
                    // rethrows the exception, preserving the original stack trace
                    ExceptionDispatchInfo.Capture(e).Throw();
                    break;
            }
        }
    }
}
