﻿using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using ManagedDismapi.Interop;

namespace ManagedDismapi {
    /// <summary>
    /// Contains functions for initializing the DISM API, mounting images, opening images for servicing, and retrieving image information.
    /// <para>You must use <see cref="Initialize"/> before calling any methods in this library. You must also call <see cref="Shutdown"/> before your program exits.</para>
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
                Utils.HandleException(e);
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
                Utils.HandleException(e);
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
                Utils.HandleException(e);
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
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the command.</param>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="mountPath"/> do not exist or are malformed.</exception>
        public static void MountWIM(string filePath, string mountPath, string imageName,
            MountOptions options, CancellationToken cancellationToken,
            IProgress<DismProgressInfo> progress) => MountImage(filePath, mountPath, 0, imageName, ImageIdentifier.Name,
            options, cancellationToken, progress);

        /// <summary>
        /// Mounts a WIM to the specified location.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="mountPath">The path to mount the image to. The path must already exist and be an empty directory on an NTFS drive.</param>
        /// <param name="imageIndex">The index of the image you want to mount.</param>
        /// <param name="options">The options to use for mounting. You must use at least <see cref="MountOptions.ReadWrite"/> or <see cref="MountOptions.ReadOnly"/>.</param>
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the command.</param>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="mountPath"/> do not exist or are malformed.</exception>
        public static void MountWIM(string filePath, string mountPath, uint imageIndex,
            MountOptions options, CancellationToken cancellationToken,
            IProgress<DismProgressInfo> progress) => MountImage(filePath, mountPath, imageIndex, null,
            ImageIdentifier.Index, options, cancellationToken, progress);

        /// <summary>
        /// Mounts a VHD or VHDX to the specified location.
        /// </summary>
        /// <param name="mountPath">The path to mount the image to. The path must already exist and be an empty directory on an NTFS drive, or an unassigned drive letter (e.g. D:).</param>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="options">The options to use for mounting. You must use at least <see cref="MountOptions.ReadWrite"/> or <see cref="MountOptions.ReadOnly"/>.</param>
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the operation.</param>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="mountPath"/> do not exist or are malformed.</exception>
        public static void MountVHD(string filePath, string mountPath, MountOptions options,
            CancellationToken cancellationToken, IProgress<DismProgressInfo> progress) => MountImage(
            filePath, mountPath, 1, null, ImageIdentifier.Index, options, cancellationToken, progress);

        internal static void MountImage(string filePath, string mountPath, uint imageIndex, string imageName,
            ImageIdentifier imageIdentifier, MountOptions options, CancellationToken cancellationToken,
            IProgress<DismProgressInfo> progress) {
            try {
                NativeMethods.DismMountImage(
                    filePath,
                    mountPath,
                    imageIndex,
                    imageName,
                    imageIdentifier,
                    options,
                    cancellationToken.WaitHandle.GetSafeWaitHandle(),
                    Utils.MakeNativeCallback(progress),
                    IntPtr.Zero
                );
            } catch(Exception e) {
                Utils.HandleException(e);
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
                Utils.HandleException(e);
            }
        }

        /// <summary>
        /// Unmounts a Windows image from a specified location.
        /// </summary>
        /// <param name="mountPath">The path to the mount point.</param>
        /// <param name="options">The options to use when unmounting the image. You must use at least <see cref="UnmountOptions.Commit"/> or <seealso cref="UnmountOptions.Discard"/>.</param>
        /// <param name="cancellationToken">A token used for canceling the operation.</param>
        /// <param name="progress">A callback called to update on the progress of the operation.</param>
        public static void UnmountImage(string mountPath, UnmountOptions options,
            CancellationToken cancellationToken, IProgress<DismProgressInfo> progress) {
            try {
                NativeMethods.DismUnmountImage(
                    mountPath,
                    options,
                    cancellationToken.WaitHandle.GetSafeWaitHandle(),
                    Utils.MakeNativeCallback(progress),
                    IntPtr.Zero
                );
            } catch(Exception e) {
                Utils.HandleException(e);
            }
        }

        /// <summary>
        /// Removes files and resources associated with corrupted or invalid mount points.
        /// </summary>
        public static void CleanupMountPoints() {
            try {
                NativeMethods.DismCleanupMountPoints();
            } catch(Exception e) {
                Utils.HandleException(e);
            }
        }

        /// <summary>
        /// Gets information about currently mounted images.
        /// </summary>
        /// <returns>An array of <see cref="MountedImageInfo"/> describing mounted images..</returns>
        public static MountedImageInfo[] GetMountedImageInfo() {
            MountedImageInfo[] mii = null;
            try {
                NativeMethods.DismGetMountedImageInfo(out IntPtr mountedImageInfo, out uint count);
                mii = Utils.MarshalArray(mountedImageInfo, count, MountedImageInfo.FromIntPtr);
                NativeMethods.DismDelete(mountedImageInfo);
            } catch(Exception e) {
                Utils.HandleException(e);
            }
            return mii;
        }

        /// <summary>
        /// Gets information about the images inside a WIM or VHD file.
        /// </summary>
        /// <param name="imagePath">The path to the image.</param>
        /// <returns>An array of <see cref="ImageInfo"/> instances.</returns>
        public static ImageInfo[] GetImageInfo(string imagePath) {
            ImageInfo[] ii = null;
            var imageInfo = IntPtr.Zero;

            try {
                NativeMethods.DismGetImageInfo(imagePath, out imageInfo, out uint count);
                ii = Utils.MarshalArray<DismImageInfo, ImageInfo>(imageInfo, count, ImageInfo.FromIntPtr);
            } catch(Exception e) {
                Utils.HandleException(e);
            } finally {
                NativeMethods.DismDelete(imageInfo);
            }

            return ii;
        }
    }
}
