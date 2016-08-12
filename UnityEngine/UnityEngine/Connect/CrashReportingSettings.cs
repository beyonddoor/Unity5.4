namespace UnityEngine.Connect
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal static class CrashReportingSettings
    {
        [ThreadAndSerializationSafe]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [ThreadAndSerializationSafe]
        public static string eventUrl { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

