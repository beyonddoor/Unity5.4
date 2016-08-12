namespace UnityEngine.Connect
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class UnityPurchasingSettings
    {
        [ThreadAndSerializationSafe]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [ThreadAndSerializationSafe]
        public static bool testMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

