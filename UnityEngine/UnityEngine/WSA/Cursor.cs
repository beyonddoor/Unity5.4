namespace UnityEngine.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Cursor
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetCustomCursor(uint id);
    }
}

