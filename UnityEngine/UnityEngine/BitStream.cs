﻿namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class BitStream
    {
        internal IntPtr m_Ptr;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Serializen(BitStream self, ref NetworkViewID viewID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Serializeq(BitStream self, ref Quaternion value, float maximumDelta);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Serializev(BitStream self, ref Vector3 value, float maximumDelta);
        public void Serialize(ref bool value)
        {
            int num = !value ? 0 : 1;
            this.Serializeb(ref num);
            value = num != 0;
        }

        public void Serialize(ref char value)
        {
            this.Serializec(ref value);
        }

        public void Serialize(ref short value)
        {
            this.Serializes(ref value);
        }

        public void Serialize(ref int value)
        {
            this.Serializei(ref value);
        }

        [ExcludeFromDocs]
        public void Serialize(ref float value)
        {
            float maxDelta = 1E-05f;
            this.Serialize(ref value, maxDelta);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Serialize(ref string value);
        public void Serialize(ref NetworkPlayer value)
        {
            int index = value.index;
            this.Serializei(ref index);
            value.index = index;
        }

        public void Serialize(ref NetworkViewID viewID)
        {
            this.Serializen(ref viewID);
        }

        [ExcludeFromDocs]
        public void Serialize(ref Quaternion value)
        {
            float maxDelta = 1E-05f;
            this.Serialize(ref value, maxDelta);
        }

        [ExcludeFromDocs]
        public void Serialize(ref Vector3 value)
        {
            float maxDelta = 1E-05f;
            this.Serialize(ref value, maxDelta);
        }

        public void Serialize(ref float value, [DefaultValue("0.00001F")] float maxDelta)
        {
            this.Serializef(ref value, maxDelta);
        }

        public void Serialize(ref Quaternion value, [DefaultValue("0.00001F")] float maxDelta)
        {
            this.Serializeq(ref value, maxDelta);
        }

        public void Serialize(ref Vector3 value, [DefaultValue("0.00001F")] float maxDelta)
        {
            this.Serializev(ref value, maxDelta);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Serializeb(ref int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Serializec(ref char value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Serializef(ref float value, float maximumDelta);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Serializei(ref int value);
        private void Serializen(ref NetworkViewID viewID)
        {
            INTERNAL_CALL_Serializen(this, ref viewID);
        }

        private void Serializeq(ref Quaternion value, float maximumDelta)
        {
            INTERNAL_CALL_Serializeq(this, ref value, maximumDelta);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Serializes(ref short value);
        private void Serializev(ref Vector3 value, float maximumDelta)
        {
            INTERNAL_CALL_Serializev(this, ref value, maximumDelta);
        }

        public bool isReading { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isWriting { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

