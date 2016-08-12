namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class LightProbeProxyVolume : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_boundsGlobal(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_originCustom(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_sizeCustom(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_originCustom(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_sizeCustom(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Update();

        public BoundingBoxMode boundingBoxMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Bounds boundsGlobal
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_boundsGlobal(out bounds);
                return bounds;
            }
        }

        public int gridResolutionX { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int gridResolutionY { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int gridResolutionZ { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isFeatureSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3 originCustom
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_originCustom(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_originCustom(ref value);
            }
        }

        public float probeDensity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ProbePositionMode probePositionMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RefreshMode refreshMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ResolutionMode resolutionMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 sizeCustom
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_sizeCustom(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_sizeCustom(ref value);
            }
        }

        public enum BoundingBoxMode
        {
            AutomaticLocal,
            AutomaticWorld,
            Custom
        }

        public enum ProbePositionMode
        {
            CellCorner,
            CellCenter
        }

        public enum RefreshMode
        {
            Automatic,
            EveryFrame,
            ViaScripting
        }

        public enum ResolutionMode
        {
            Automatic,
            Custom
        }
    }
}

