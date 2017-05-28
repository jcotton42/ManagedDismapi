using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace ManagedDismapi {
    internal static class Utils {
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

        internal static T[] MarshalArray<T>(IntPtr ptr, uint count, Func<IntPtr, T> marshaller) {
            var size = Marshal.SizeOf<T>();
            var array = new T[count];
            for(uint i = 0; i < count; i++) {
                array[i] = marshaller(ptr);
                ptr += size;
            }
            return array;
        }

        internal static void PrepareCallbackAndUserData(object userData, IProgress<DismProgressInfo> progress,
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
    }
}
