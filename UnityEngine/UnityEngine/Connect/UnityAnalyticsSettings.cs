namespace UnityEngine.Connect
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class UnityAnalyticsSettings
    {
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

