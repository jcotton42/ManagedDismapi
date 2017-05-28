using System;
using System.Runtime.InteropServices;

namespace ManagedDismapi {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public sealed class MountedImageInfo {
        public string MountPath { get; }
        public string ImageFilePath { get; }
        public uint ImageIndex { get; }
        public MountMode MountMode { get; }
        public MountStatus MountStatus { get; }

        internal static MountedImageInfo FromIntPtr(IntPtr ptr) {
            return Marshal.PtrToStructure<MountedImageInfo>(ptr);
        }
    }
}
