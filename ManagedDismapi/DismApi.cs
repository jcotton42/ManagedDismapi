using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

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

        /// <summary>
        /// Opens the running OS image for servicing.
        /// </summary>
        /// <returns>A new <see cref="WindowsImage"/> corresponding to the running OS image.</returns>
        /// <exception cref="TODO">TODO</exception>
        public static WindowsImage OpenOSImage() => OpenImage(NativeMethods.DISM_ONLINE_IMAGE, null, null);

        /// <summary>
        /// Opens an offline Windows image for servicing.
        /// </summary>
        /// <param name="imagePath">An absolute or relative path to the directory of an offline Windows image.</param>
        /// <returns>A new <see cref="WindowsImage"/> corresponding to the offline image.</returns>
        /// <exception cref="TODO">TODO</exception>
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

        /// <summary>
        /// Mounts a WIM to the specified location.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="mountPath">The path to mount the image to. The path must already exist and be an empty directory on an NTFS drive.</param>
        /// <param name="imageName">The name of the image you want to mount.</param>
        /// <param name="options">The options to use for mounting. You must use at least <see cref="MountOptions.ReadWrite"/> or <see cref="MountOptions.ReadOnly"/>.</param>
        /// <param name="userData">The object passed to <paramref name="progress"/> callback.</param>
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the command.</param>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="mountPath"/> do not exist or are malformed.</exception>
        public static void MountWIM(string filePath, string mountPath, string imageName,
            MountOptions options, object userData, CancellationToken cancellationToken,
            IProgress<DismProgressInfo> progress) => MountImage(filePath, mountPath, 0, imageName, ImageIdentifier.Name,
            options, userData, cancellationToken, progress);

        /// <summary>
        /// Mounts a WIM to the specified location.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="mountPath">The path to mount the image to. The path must already exist and be an empty directory on an NTFS drive.</param>
        /// <param name="imageIndex">The index of the image you want to mount.</param>
        /// <param name="options">The options to use for mounting. You must use at least <see cref="MountOptions.ReadWrite"/> or <see cref="MountOptions.ReadOnly"/>.</param>
        /// <param name="userData">The object passed to <paramref name="progress"/> callback.</param>
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the command.</param>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="mountPath"/> do not exist or are malformed.</exception>
        public static void MountWIM(string filePath, string mountPath, uint imageIndex,
            MountOptions options, object userData, CancellationToken cancellationToken,
            IProgress<DismProgressInfo> progress) => MountImage(filePath, mountPath, imageIndex, null,
            ImageIdentifier.Index, options, userData, cancellationToken, progress);

        /// <summary>
        /// Mounts a VHD or VHDX to the specified location.
        /// </summary>
        /// <param name="mountPath">The path to mount the image to. The path must already exist and be an empty directory on an NTFS drive, or an unassigned drive letter (e.g. D:).</param>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="options">The options to use for mounting. You must use at least <see cref="MountOptions.ReadWrite"/> or <see cref="MountOptions.ReadOnly"/>.</param>
        /// <param name="userData">The object passed to <paramref name="progress"/> callback.</param>
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the operation.</param>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="mountPath"/> do not exist or are malformed.</exception>
        public static void MountVHD(string filePath, string mountPath, MountOptions options,
            object userData, CancellationToken cancellationToken, IProgress<DismProgressInfo> progress) => MountImage(
            filePath, mountPath, 1, null, ImageIdentifier.Index, options, userData, cancellationToken, progress);

        internal static void MountImage(string filePath, string mountPath, uint imageIndex, string imageName,
            ImageIdentifier imageIdentifier, MountOptions options, object userData,
            CancellationToken cancellationToken, IProgress<DismProgressInfo> progress) {
            PrepareCallbackAndUserData(userData, progress, out IntPtr ptr, out DismProgressCallback dpc);

            try {
                NativeMethods.DismMountImage(
                    filePath,
                    mountPath,
                    imageIndex,
                    imageName,
                    imageIdentifier,
                    options,
                    cancellationToken.WaitHandle.GetSafeWaitHandle(),
                    dpc,
                    ptr
                );
            } catch(Exception e) {
                HandleHResult(e);
            } finally {
                if(ptr != IntPtr.Zero) {
                    GCHandle.FromIntPtr(ptr).Free();
                }
            }
        }

        /// <summary>
        /// Remounts a previously mounted Windows image at the specified path.
        /// </summary>
        /// <param name="mountPath">An absolute or relative path to the mount directory of the image.</param>
        public static void RemountImage(string mountPath) {
            try {
                NativeMethods.DismRemountImage(mountPath);
            } catch(Exception e) {
                HandleHResult(e);
            }
        }

        /// <summary>
        /// Unmounts a Windows image from a specified location.
        /// </summary>
        /// <param name="mountPath">The path to the mount point.</param>
        /// <param name="options">The options to use when unmounting the image. You must use at least <see cref="UnmountOptions.Commit"/> or <seealso cref="UnmountOptions.Discard"/>.</param>
        /// <param name="userData">The object passed to <paramref name="progress"/> callback.</param>
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the operation.</param>
        public static void UnmountImage(string mountPath, UnmountOptions options, object userData,
            CancellationToken cancellationToken, IProgress<DismProgressInfo> progress) {
            PrepareCallbackAndUserData(userData, progress, out IntPtr ptr, out DismProgressCallback dpc);

            try {
                NativeMethods.DismUnmountImage(
                    mountPath,
                    options,
                    cancellationToken.WaitHandle.GetSafeWaitHandle(),
                    dpc,
                    ptr
                );
            } catch(Exception e) {
                HandleHResult(e);
            } finally {
                if(ptr != IntPtr.Zero) {
                    GCHandle.FromIntPtr(ptr).Free();
                }
            }
        }

        /// <summary>
        /// Removes files and resources associated with corrupted or invalid mount points.
        /// </summary>
        public static void CleanupMountPoints() {
            try {
                NativeMethods.DismCleanupMountPoints();
            } catch(Exception e) {
                HandleHResult(e);
            }
        }

        private static void PrepareCallbackAndUserData(object userData, IProgress<DismProgressInfo> progress,
            out IntPtr ptr, out DismProgressCallback dpc) {
            ptr = IntPtr.Zero;
            if(userData != null) {
                userData = GCHandle.ToIntPtr(GCHandle.Alloc(userData));
            }

            dpc = null;
            if(progress != null) {
                dpc = (current, total, data) => progress.Report(new DismProgressInfo(current, total, data));
            }
        }

        internal static void HandleHResult(Exception e) {
            NativeMethods.DismGetLastErrorMessage(out IntPtr ptr);
            var message = DismString.FromIntPtr(ptr).Value;
            NativeMethods.DismDelete(ptr);

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
