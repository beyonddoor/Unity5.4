namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class MaterialPropertyBlock
    {
        internal IntPtr m_Ptr;

        public MaterialPropertyBlock()
        {
            this.InitBlock();
        }

        [Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
        public void AddColor(int nameID, Color value)
        {
            this.SetColor(nameID, value);
        }

        [Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
        public void AddColor(string name, Color value)
        {
            this.SetColor(Shader.PropertyToID(name), value);
        }

        [Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
        public void AddFloat(int nameID, float value)
        {
            this.SetFloat(nameID, value);
        }

        [Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
        public void AddFloat(string name, float value)
        {
            this.SetFloat(Shader.PropertyToID(name), value);
        }

        [Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
        public void AddMatrix(int nameID, Matrix4x4 value)
        {
            this.SetMatrix(nameID, value);
        }

        [Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
        public void AddMatrix(string name, Matrix4x4 value)
        {
            this.SetMatrix(Shader.PropertyToID(name), value);
        }

        [Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
        public void AddTexture(int nameID, Texture value)
        {
            this.SetTexture(nameID, value);
        }

        [Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
        public void AddTexture(string name, Texture value)
        {
            this.SetTexture(Shader.PropertyToID(name), value);
        }

        [Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
        public void AddVector(int nameID, Vector4 value)
        {
            this.SetVector(nameID, value);
        }

        [Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
        public void AddVector(string name, Vector4 value)
        {
            this.SetVector(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        internal extern void DestroyBlock();
        ~MaterialPropertyBlock()
        {
            this.DestroyBlock();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetFloat(int nameID);
        public float GetFloat(string name)
        {
            return this.GetFloat(Shader.PropertyToID(name));
        }

        public Matrix4x4 GetMatrix(int nameID)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetMatrix(this, nameID, out matrixx);
            return matrixx;
        }

        public Matrix4x4 GetMatrix(string name)
        {
            return this.GetMatrix(Shader.PropertyToID(name));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Texture GetTexture(int nameID);
        public Texture GetTexture(string name)
        {
            return this.GetTexture(Shader.PropertyToID(name));
        }

        public Vector4 GetVector(int nameID)
        {
            Vector4 vector;
            INTERNAL_CALL_GetVector(this, nameID, out vector);
            return vector;
        }

        public Vector4 GetVector(string name)
        {
            return this.GetVector(Shader.PropertyToID(name));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InitBlock();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetMatrix(MaterialPropertyBlock self, int nameID, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetVector(MaterialPropertyBlock self, int nameID, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetColor(MaterialPropertyBlock self, int nameID, ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetMatrix(MaterialPropertyBlock self, int nameID, ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetVector(MaterialPropertyBlock self, int nameID, ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetBuffer(int nameID, ComputeBuffer value);
        public void SetBuffer(string name, ComputeBuffer value)
        {
            this.SetBuffer(Shader.PropertyToID(name), value);
        }

        public void SetColor(int nameID, Color value)
        {
            INTERNAL_CALL_SetColor(this, nameID, ref value);
        }

        public void SetColor(string name, Color value)
        {
            this.SetColor(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetFloat(int nameID, float value);
        public void SetFloat(string name, float value)
        {
            this.SetFloat(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetFloatArray(int nameID, float[] values);
        public void SetFloatArray(string name, float[] values)
        {
            this.SetFloatArray(Shader.PropertyToID(name), values);
        }

        public void SetMatrix(int nameID, Matrix4x4 value)
        {
            INTERNAL_CALL_SetMatrix(this, nameID, ref value);
        }

        public void SetMatrix(string name, Matrix4x4 value)
        {
            this.SetMatrix(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetMatrixArray(int nameID, Matrix4x4[] values);
        public void SetMatrixArray(string name, Matrix4x4[] values)
        {
            this.SetMatrixArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTexture(int nameID, Texture value);
        public void SetTexture(string name, Texture value)
        {
            this.SetTexture(Shader.PropertyToID(name), value);
        }

        public void SetVector(int nameID, Vector4 value)
        {
            INTERNAL_CALL_SetVector(this, nameID, ref value);
        }

        public void SetVector(string name, Vector4 value)
        {
            this.SetVector(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetVectorArray(int nameID, Vector4[] values);
        public void SetVectorArray(string name, Vector4[] values)
        {
            this.SetVectorArray(Shader.PropertyToID(name), values);
        }

        public bool isEmpty { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

