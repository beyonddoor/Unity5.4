﻿namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class LightProbeGroup : Behaviour
    {
        public Vector3[] probePositions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

