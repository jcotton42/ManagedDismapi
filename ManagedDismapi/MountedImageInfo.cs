using System;
using System.Runtime.InteropServices;

namespace ManagedDismapi {
    /// <summary>
    /// Information about a mounted Windows image.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public sealed class MountedImageInfo {
        internal MountedImageInfo() { }

        /// <summary>
        /// The path to the mount point of the image.
        /// </summary>
        public string MountPath { get; }

        /// <summary>
        /// The path to the file containing this image.
        /// </summary>
        public string ImageFilePath { get; }

        /// <summary>
        /// The 1-based index of this image in the image file.
        /// </summary>
        public uint ImageIndex { get; }

        /// <summary>
        /// The way this image is mounted.
        /// </summary>
        public MountMode MountMode { get; }

        /// <summary>
        /// The current mount status of the image.
        /// </summary>
        public MountStatus MountStatus { get; }

        internal static MountedImageInfo FromIntPtr(IntPtr ptr) {
            return Marshal.PtrToStructure<MountedImageInfo>(ptr);
        }
    }
}
