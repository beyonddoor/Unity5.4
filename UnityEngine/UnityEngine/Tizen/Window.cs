namespace UnityEngine.Tizen
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class Window
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_windowHandle(out IntPtr value);

        public static IntPtr windowHandle
        {
            get
            {
                IntPtr ptr;
                INTERNAL_get_windowHandle(out ptr);
                return ptr;
            }
        }
    }
}

