namespace UnityEngine.Diagnostics
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class PlayerConnection
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SendFile(string remoteFilePath, byte[] data);

        public static bool connected { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

