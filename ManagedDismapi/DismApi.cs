﻿using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
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
