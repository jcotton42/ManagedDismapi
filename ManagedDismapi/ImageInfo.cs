using System;
using System.Linq;
using System.Runtime.InteropServices;
using ManagedDismapi.Interop;

namespace ManagedDismapi {
    /// <summary>
    /// Metadata for a Windows image.
    /// </summary>
    public sealed class ImageInfo {
        internal ImageInfo() { }

        /// <summary>
        /// The type of image, such as WIM or VHD.
        /// </summary>
        public ImageType ImageType { get; private set; }

        /// <summary>
        /// The 1-based index of the image in the file.
        /// </summary>
        public uint ImageIndex { get; private set; }

        /// <summary>
        /// The name of the image.
        /// </summary>
        public string ImageName { get; private set; }

        /// <summary>
        /// The description of the image.
        /// </summary>
        public string ImageDescription { get; private set; }

        /// <summary>
        /// The size of the image in bytes.
        /// </summary>
        public ulong ImageSize { get; private set; }

        /// <summary>
        /// The architecture of the image.
        /// </summary>
        public ImageArchitecture Architecture { get; private set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        public string ProductName { get; private set; }

        /// <summary>
        /// The edition of the product.
        /// </summary>
        public string EditionId { get; private set; }

        /// <summary>
        /// The installation type of the image.
        /// </summary>
        public string InstallationType { get; private set; }

        /// <summary>
        /// The HAL type of the operation system.
        /// </summary>
        /// <remarks>Will not be set on generalized images.</remarks>
        public string HAL { get; private set; }

        /// <summary>
        /// The product type.
        /// </summary>
        public string ProductType { get; private set; }

        /// <summary>
        /// The product suite.
        /// </summary>
        public string ProductSuite { get; private set; }

        /// <summary>
        /// The version of the operating system in the image.
        /// </summary>
        public Version Version { get; private set; }

        /// <summary>
        /// The service pack build of the operating system.
        /// </summary>
        public uint ServicePackBuild { get; private set; }

        /// <summary>
        /// The service pack level of the operating system.
        /// </summary>
        public uint ServicePackLevel { get; private set; }

        /// <summary>
        /// The bootable status of the image.
        /// </summary>
        public ImageBootable Bootable { get; private set; }

        /// <summary>
        /// The relative path to the Windows directory.
        /// </summary>
        public string SystemRoot { get; private set; }

        /// <summary>
        /// The languages in the image.
        /// </summary>
        public string[] Languages { get; private set; }

        /// <summary>
        /// The default language in the image.
        /// </summary>
        public string DefaultLanguage { get; private set; }

        /// <summary>
        /// Additional information for WIM images.
        /// </summary>
        public WIMCustomizedInfo CustomizedInfo { get; private set; }

        internal static ImageInfo FromIntPtr(IntPtr ptr) {
            var dii = Marshal.PtrToStructure<DismImageInfo>(ptr);
            var info = new ImageInfo {
                ImageType = dii.ImageType,
                ImageIndex = dii.ImageIndex,
                ImageName = dii.ImageName,
                ImageDescription = dii.ImageDescription,
                ImageSize = dii.ImageSize,
                Architecture = dii.Architecture,
                ProductName = dii.ProductName,
                EditionId = dii.EditionId,
                InstallationType = dii.InstallationType,
                HAL = dii.HAL,
                ProductType = dii.ProductType,
                ProductSuite = dii.ProductSuite,
                Version = new Version((int)dii.MajorVersion, (int)dii.MinorVersion, (int)dii.Build, (int)dii.SpBuild),
                ServicePackBuild = dii.SpBuild,
                ServicePackLevel = dii.SpLevel,
                Bootable = dii.Bootable,
                SystemRoot = dii.SystemRoot,
                Languages = Utils.MarshalArray(dii.Languages, dii.LanguageCount, DismString.FromIntPtr)
                    .Select(ds => ds.Value).ToArray()
            };

            info.DefaultLanguage = info.Languages[dii.DefaultLanguageIndex];

            if(dii.CustomizedInfo != IntPtr.Zero) {
                info.CustomizedInfo = WIMCustomizedInfo.FromIntPtr(dii.CustomizedInfo);
            }

            return info;
        }
    }
}
