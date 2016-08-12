namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class EventProvider
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void WriteCustomEvent(int value, string text);
    }
}

