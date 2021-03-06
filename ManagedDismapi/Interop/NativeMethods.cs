﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

#if DEBUG
[assembly: InternalsVisibleTo("LINQPadQuery")]
#endif

[module: DefaultCharSet(CharSet.Unicode)]

namespace ManagedDismapi.Interop {
    internal static class NativeMethods {
        #region Constants

        private const string DismApi = "DismApi";

        internal const int ERROR_ELEVATION_REQUIRED_HR = unchecked((int)0x800702E4);
        internal const int ERROR_SUCCESS_REBOOT_REQUIRED = 0x00000BC2;

        internal const int DISMAPI_S_RELOAD_IMAGE_SESSION_REQUIRED = 0x00000001;

        internal const int DISMAPI_E_DISMAPI_NOT_INITIALIZED = unchecked((int)0xC0040001);
        internal const int DISMAPI_E_SHUTDOWN_IN_PROGRESS = unchecked((int)0xC0040002);
        internal const int DISMAPI_E_OPEN_SESSION_HANDLES = unchecked((int)0xC0040003);
        internal const int DISMAPI_E_INVALID_DISM_SESSION = unchecked((int)0xC0040004);
        internal const int DISMAPI_E_INVALID_IMAGE_INDEX = unchecked((int)0xC0040005);
        internal const int DISMAPI_E_INVALID_IMAGE_NAME = unchecked((int)0xC0040006);
        internal const int DISMAPI_E_UNABLE_TO_UNMOUNT_IMAGE_PATH = unchecked((int)0xC0040007);
        internal const int DISMAPI_E_LOGGING_DISABLED = unchecked((int)0xC0040009);
        internal const int DISMAPI_E_OPEN_HANDLES_UNABLE_TO_UNMOUNT_IMAGE_PATH = unchecked((int)0xC004000A);
        internal const int DISMAPI_E_OPEN_HANDLES_UNABLE_TO_MOUNT_IMAGE_PATH = unchecked((int)0xC004000B);
        internal const int DISMAPI_E_OPEN_HANDLES_UNABLE_TO_REMOUNT_IMAGE_PATH = unchecked((int)0xC004000C);
        internal const int DISMAPI_E_PARENT_FEATURE_DISABLED = unchecked((int)0xC004000D);
        internal const int DISMAPI_E_MUST_SPECIFY_ONLINE_IMAGE = unchecked((int)0xC004000E);
        internal const int DISMAPI_E_INVALID_PRODUCT_KEY = unchecked((int)0xC004000F);
        internal const int DISMAPI_E_NEEDS_REMOUNT = unchecked((int)0XC1510114);
        internal const int DISMAPI_E_UNKNOWN_FEATURE = unchecked((int)0x800f080c);
        internal const int DISMAPI_E_BUSY = unchecked((int)0x800f0902);

        internal const string DISM_ONLINE_IMAGE = "DISM_{53BFAE52-B167-4E2F-A258-0A37B57FF845}";
        internal const uint DISM_SESSION_DEFAULT = 0;

        internal const uint DISM_MOUNT_READWRITE = 0x00000000;
        internal const uint DISM_MOUNT_READONLY = 0x00000001;
        internal const uint DISM_MOUNT_OPTIMIZE = 0x00000002;
        internal const uint DISM_MOUNT_CHECK_INTEGRITY = 0x00000004;

        internal const uint DISM_COMMIT_IMAGE = 0x00000000;
        internal const uint DISM_DISCARD_IMAGE = 0x00000001;

        internal const uint DISM_COMMIT_GENERATE_INTEGRITY = 0x00010000;
        internal const uint DISM_COMMIT_APPEND = 0x00020000;

        #endregion

        // TODO: find the value of DISMAPI_E_DISMAPI_ALREADY_INITIALIZED

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismCleanupMountPoints();

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismCloseSession(
            uint session
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismCommitImage(
            uint session,
            CommitOptions flags,
            SafeWaitHandle cancelEvent,
            DismProgressCallback progress,
            IntPtr userData
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismDelete(
            IntPtr ptr
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismGetImageInfo(
            string imageFilePath,
            out IntPtr imageInfo,
            out uint count
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismGetLastErrorMessage(
            out IntPtr message
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismGetMountedImageInfo(
            out IntPtr mountedImageInfo,
            out uint count
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismInitialize(
            LogLevel logLevel,
            string logPath,
            string scratchPath
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismMountImage(
            string filePath,
            string mountPath,
            uint imageIndex,
            string imageName,
            ImageIdentifier imageIdentifier,
            MountOptions options,
            SafeWaitHandle cancelEvent,
            DismProgressCallback progress,
            IntPtr userData
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismOpenSession(
            string imagePath,
            string windowsDirectory,
            string systemDrive,
            out uint session
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismRemountImage(
            string mountPath
        );

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismShutdown();

        [DllImport(DismApi, PreserveSig = false)]
        internal static extern void DismUnmountImage(
            string mountPath,
            UnmountOptions options,
            SafeWaitHandle cancelEvent,
            DismProgressCallback progress,
            IntPtr userData
        );
    }

    internal delegate void DismProgressCallback(uint current, uint total, IntPtr userData);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    internal sealed class DismString {
        internal string Value;

        internal static DismString FromIntPtr(IntPtr ptr) {
            return Marshal.PtrToStructure<DismString>(ptr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SYSTEMTIME {
        internal ushort Year;
        internal ushort Month;
        internal ushort DayOfWeek;
        internal ushort Day;
        internal ushort Hour;
        internal ushort Minute;
        internal ushort Second;
        internal ushort Millisecond;

        internal DateTime ToDateTime() => new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
    }
}
