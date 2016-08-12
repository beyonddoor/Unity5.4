namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    public sealed class Texture2DArray : Texture
    {
        public Texture2DArray(int width, int height, int depth, TextureFormat format, bool mipmap)
        {
            Internal_Create(this, width, height, depth, format, mipmap, false);
        }

        public Texture2DArray(int width, int height, int depth, TextureFormat format, bool mipmap, bool linear)
        {
            Internal_Create(this, width, height, depth, format, mipmap, linear);
        }

        [ExcludeFromDocs]
        public void Apply()
        {
            bool makeNoLongerReadable = false;
            bool updateMipmaps = true;
            this.Apply(updateMipmaps, makeNoLongerReadable);
        }

        [ExcludeFromDocs]
        public void Apply(bool updateMipmaps)
        {
            bool makeNoLongerReadable = false;
            this.Apply(updateMipmaps, makeNoLongerReadable);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable);
        [ExcludeFromDocs]
        public Color[] GetPixels(int arrayElement)
        {
            int miplevel = 0;
            return this.GetPixels(arrayElement, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color[] GetPixels(int arrayElement, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public Color32[] GetPixels32(int arrayElement)
        {
            int miplevel = 0;
            return this.GetPixels32(arrayElement, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color32[] GetPixels32(int arrayElement, [DefaultValue("0")] int miplevel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create([Writable] Texture2DArray mono, int width, int height, int depth, TextureFormat format, bool mipmap, bool linear);
        [ExcludeFromDocs]
        public void SetPixels(Color[] colors, int arrayElement)
        {
            int miplevel = 0;
            this.SetPixels(colors, arrayElement, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPixels(Color[] colors, int arrayElement, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public void SetPixels32(Color32[] colors, int arrayElement)
        {
            int miplevel = 0;
            this.SetPixels32(colors, arrayElement, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPixels32(Color32[] colors, int arrayElement, [DefaultValue("0")] int miplevel);

        public int depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

