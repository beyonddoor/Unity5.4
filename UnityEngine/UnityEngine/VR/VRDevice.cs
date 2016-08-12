namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class VRDevice
    {
        public static IntPtr GetNativePtr()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativePtr(out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetNativePtr(out IntPtr value);

        [Obsolete("family is deprecated.  Use VRSettings.loadedDeviceName instead.")]
        public static string family { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isPresent { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string model { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float refreshRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

