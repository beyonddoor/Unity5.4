namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class ComputeShader : UnityEngine.Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispatch(int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);
        [ExcludeFromDocs]
        public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer)
        {
            uint argsOffset = 0;
            this.DispatchIndirect(kernelIndex, argsBuffer, argsOffset);
        }

        public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer, [DefaultValue("0")] uint argsOffset)
        {
            if (argsBuffer == null)
            {
                throw new ArgumentNullException("argsBuffer");
            }
            if (argsBuffer.m_Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("argsBuffer");
            }
            this.Internal_DispatchIndirect(kernelIndex, argsBuffer, argsOffset);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int FindKernel(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void GetKernelThreadGroupSizes(int kernelIndex, out uint x, out uint y, out uint z);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool HasKernel(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetVector(ComputeShader self, string name, ref Vector4 val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer, uint argsOffset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetFloats(string name, float[] values);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetInts(string name, int[] values);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetBuffer(int kernelIndex, string name, ComputeBuffer buffer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetFloat(string name, float val);
        public void SetFloats(string name, params float[] values)
        {
            this.Internal_SetFloats(name, values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetInt(string name, int val);
        public void SetInts(string name, params int[] values)
        {
            this.Internal_SetInts(name, values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTexture(int kernelIndex, string name, Texture texture);
        public void SetVector(string name, Vector4 val)
        {
            INTERNAL_CALL_SetVector(this, name, ref val);
        }
    }
}

