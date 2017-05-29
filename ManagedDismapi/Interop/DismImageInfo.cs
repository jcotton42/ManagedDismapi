using System;
using System.Runtime.InteropServices;

namespace ManagedDismapi.Interop {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    internal sealed class DismImageInfo {
        internal ImageType ImageType;
        internal uint ImageIndex;
        internal string ImageName;
        internal string ImageDescription;
        internal ulong ImageSize;
        internal ImageArchitecture Architecture;
        internal string ProductName;
        internal string EditionId;
        internal string InstallationType;
        internal string HAL;
        internal string ProductType;
        internal string ProductSuite;
        internal uint MajorVersion;
        internal uint MinorVersion;
        internal uint Build;
        internal uint SpBuild;
        internal uint SpLevel;
        internal ImageBootable Bootable;
        internal string SystemRoot;
        internal IntPtr Languages;
        internal uint LanguageCount;
        internal uint DefaultLanguageIndex;
        internal IntPtr CustomizedInfo;
    }
}
