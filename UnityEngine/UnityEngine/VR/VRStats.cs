namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class VRStats
    {
        public static float gpuTimeLastFrame { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

