namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    public class Texture : UnityEngine.Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use GetNativeTexturePtr instead."), WrapperlessIcall]
        public extern int GetNativeTextureID();
        public IntPtr GetNativeTexturePtr()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativeTexturePtr(this, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetNativeTexturePtr(Texture self, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_texelSize(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern TextureDimension Internal_GetDimension(Texture t);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetHeight(Texture t);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetWidth(Texture t);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetGlobalAnisotropicFilteringLimits(int forcedMin, int globalMax);

        public int anisoLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static AnisotropicFiltering anisotropicFiltering { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public virtual TextureDimension dimension
        {
            get
            {
                return Internal_GetDimension(this);
            }
            set
            {
                throw new Exception("not implemented");
            }
        }

        public FilterMode filterMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public virtual int height
        {
            get
            {
                return Internal_GetHeight(this);
            }
            set
            {
                throw new Exception("not implemented");
            }
        }

        public static int masterTextureLimit { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float mipMapBias { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector2 texelSize
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_texelSize(out vector);
                return vector;
            }
        }

        public virtual int width
        {
            get
            {
                return Internal_GetWidth(this);
            }
            set
            {
                throw new Exception("not implemented");
            }
        }

        public TextureWrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

