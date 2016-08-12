namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class AndroidInput
    {
        private AndroidInput()
        {
        }

        public static Touch GetSecondaryTouch(int index)
        {
            Touch touch;
            INTERNAL_CALL_GetSecondaryTouch(index, out touch);
            return touch;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetSecondaryTouch(int index, out Touch value);

        public static bool secondaryTouchEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int secondaryTouchHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int secondaryTouchWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int touchCountSecondary { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

