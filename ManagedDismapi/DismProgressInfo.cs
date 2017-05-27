using System;
using System.Runtime.InteropServices;

namespace ManagedDismapi {
    /// <summary>
    /// The progress information passed during a DISM operation.
    /// </summary>
    public sealed class DismProgressInfo {
        internal DismProgressInfo(uint current, uint total, IntPtr userData) {
            Current = current;
            Total = total;

            if(userData != IntPtr.Zero) {
                UserData = GCHandle.FromIntPtr(userData).Target;
            }
        }

        /// <summary>
        /// The current progress value.
        /// </summary>
        public uint Current { get; }

        /// <summary>
        /// The total progress value.
        /// </summary>
        public uint Total { get; }

        /// <summary>
        /// User-defined custom data.
        /// </summary>
        public object UserData { get; }
    }
}
