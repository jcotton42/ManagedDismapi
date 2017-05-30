using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using ManagedDismapi.Interop;

namespace ManagedDismapi {
    internal static class Utils {
        internal static void HandleException(Exception e) {
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
            return MarshalArray<T, T>(ptr, count, marshaller);
        }

        internal static TManaged[] MarshalArray<TUnmanaged, TManaged>(IntPtr ptr, uint count, Func<IntPtr, TManaged> marshaller) {
            var size = Marshal.SizeOf<TUnmanaged>();
            var array = new TManaged[count];
            for(uint i = 0; i < count; i++) {
                array[i] = marshaller(ptr);
                ptr += size;
            }
            return array;
        }

        internal static DismProgressCallback MakeNativeCallback(IProgress<DismProgressInfo> progress) {
            if(progress != null) {
                return (current, total, _) => progress.Report(new DismProgressInfo(current, total));
            }
            return null;
        }
    }
}
