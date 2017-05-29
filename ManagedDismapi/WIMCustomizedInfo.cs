using System;
using System.Runtime.InteropServices;
using ManagedDismapi.Interop;

namespace ManagedDismapi {
    /// <summary>
    /// Describes a WIM file.
    /// </summary>
    public sealed class WIMCustomizedInfo {
        internal WIMCustomizedInfo() { }

        /// <summary>
        /// The number of directories in the image.
        /// </summary>
        public uint DirectoryCount { get; private set; }

        /// <summary>
        /// The number of files in the image.
        /// </summary>
        public uint FileCount { get; private set; }

        /// <summary>
        /// The time the image file was created.
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// The time the image was last modified.
        /// </summary>
        public DateTime LastModifiedTime { get; private set; }

        internal static WIMCustomizedInfo FromIntPtr(IntPtr ptr) {
            var info = new WIMCustomizedInfo();

            // skip Size member, which is the size of the structure, and therefore not needed
            ptr += Marshal.SizeOf<uint>();

            info.DirectoryCount = (uint)Marshal.ReadInt32(ptr);
            ptr += Marshal.SizeOf<uint>();

            info.FileCount = (uint)Marshal.ReadInt32(ptr);
            ptr += Marshal.SizeOf<uint>();

            info.CreationTime = Marshal.PtrToStructure<SYSTEMTIME>(ptr).ToDateTime();
            ptr += Marshal.SizeOf<SYSTEMTIME>();

            info.LastModifiedTime = Marshal.PtrToStructure<SYSTEMTIME>(ptr).ToDateTime();

            return info;
        }
    }
}
