using System.Runtime.InteropServices;

namespace ManagedDismapi {
    public static class DismApi {
        /// <summary>
        /// Initializes the DISM API for this process.
        /// </summary>
        /// <param name="logLevel">The logging level.</param>
        /// <param name="logPath">Relative or absolute path to a log file. Defaults to %windir%\Logs\DISM\dism.log if <c>null</c> is passed.</param>
        /// <param name="scratchPath">Relative or absolute path to a scratch directory. Defaults to %windir%\Temp if <c>null</c> is passed.</param>
        /// <exception cref="TODO">TODO</exception>
        public static void Initialize(LogLevel logLevel, string logPath, string scratchPath) {
            HandleHResult(NativeMethods.DismInitialize(logLevel, logPath, scratchPath));
        }

        /// <summary>
        /// Shuts down the DISM API for this process.
        /// </summary>
        /// <exception cref="TODO">TODO</exception>
        public static void Shutdown() {
            HandleHResult(NativeMethods.DismShutdown());
        }
        
        internal static void HandleHResult(int hr) {
            switch(hr) {
                //TODO
                default:
                    if(hr < 0) {
                        throw Marshal.GetExceptionForHR(hr);
                    }
                    return;
            }
        }
    }
}
