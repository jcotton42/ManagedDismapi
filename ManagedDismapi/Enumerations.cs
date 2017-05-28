using System;

namespace ManagedDismapi {
    internal enum ImageIdentifier {
        Index = 0,
        Name = 1
    }

    /// <summary>
    /// The logging level used by DISM operations.
    /// </summary>
    public enum LogLevel {
        /// <summary>
        /// Log only errors.
        /// </summary>
        Errors = 0,

        /// <summary>
        /// Log only errors and warnings.
        /// </summary>
        ErrorsWarnings = 1,

        /// <summary>
        /// Log errors, warnings, and additional information.
        /// </summary>
        ErrorsWarningsInfo = 2
    }

    /// <summary>
    /// The way an image was mounted.
    /// </summary>
    public enum MountMode {
        /// <summary>
        /// The image was mounted readwrite.
        /// </summary>
        ReadWrite = 0,
        /// <summary>
        /// The image was mounted readonly.
        /// </summary>
        ReadOnly = 1
    }

    /// <summary>
    /// Options to use when mounting a Windows image.
    /// </summary>
    [Flags]
    public enum MountOptions {
        /// <summary>
        /// Mounts the image read/write.
        /// </summary>
        ReadWrite = NativeMethods.DISM_MOUNT_READWRITE,

        /// <summary>
        /// Mounts the image readonly.
        /// </summary>
        ReadOnly = NativeMethods.DISM_MOUNT_READONLY,

        /// <summary>
        /// Mounts the image in optimize mode, where each directory is read from the image upon access, rather than at mount time.
        /// </summary>
        Optimize = NativeMethods.DISM_MOUNT_OPTIMIZE,

        /// <summary>
        /// Stop mounting if the image is determined to be corrupt. Does not apply when mounting VHDs.
        /// </summary>
        CheckIntegrity = NativeMethods.DISM_MOUNT_CHECK_INTEGRITY
    }

    /// <summary>
    /// The mount status of an image.
    /// </summary>
    public enum MountStatus {
        /// <summary>
        /// The image is mounted and ready for servicing.
        /// </summary>
        OK = 0,
        /// <summary>
        /// The image must be remounted before being serviced.
        /// </summary>
        NeedsRemount = 1,
        /// <summary>
        /// The image is corrupt and in an invalid state.
        /// </summary>
        Invalid = 2
    }

    /// <summary>
    /// Options to use when unmount a Windows image.
    /// </summary>
    [Flags]
    public enum UnmountOptions {
        /// <summary>
        /// Save changes to the image.
        /// </summary>
        Commit = NativeMethods.DISM_COMMIT_IMAGE,

        /// <summary>
        /// Discard changes to the image.
        /// </summary>
        Discard = NativeMethods.DISM_DISCARD_IMAGE,

        /// <summary>
        /// Saves integrity information into the image. Must be used with <see cref="Commit"/>. Does not apply to VHDs.
        /// </summary>
        //TODO: are the last two parts true?
        GenerateIntegrity = NativeMethods.DISM_COMMIT_GENERATE_INTEGRITY,

        /// <summary>
        /// Append the changes into a new image in the WIM. Does not apply to VHDs.
        /// </summary>
        Append = NativeMethods.DISM_COMMIT_APPEND
    }
}
