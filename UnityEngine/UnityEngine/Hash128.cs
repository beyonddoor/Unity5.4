﻿namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Hash128
    {
        private uint m_u32_0;
        private uint m_u32_1;
        private uint m_u32_2;
        private uint m_u32_3;
        public Hash128(uint u32_0, uint u32_1, uint u32_2, uint u32_3)
        {
            this.m_u32_0 = u32_0;
            this.m_u32_1 = u32_1;
            this.m_u32_2 = u32_2;
            this.m_u32_3 = u32_3;
        }

        public bool isValid
        {
            get
            {
                return ((((this.m_u32_0 != 0) || (this.m_u32_1 != 0)) || (this.m_u32_2 != 0)) || (this.m_u32_3 != 0));
            }
        }
        public override string ToString()
        {
            return Internal_Hash128ToString(this.m_u32_0, this.m_u32_1, this.m_u32_2, this.m_u32_3);
        }

        public static Hash128 Parse(string hashString)
        {
            Hash128 hash;
            INTERNAL_CALL_Parse(hashString, out hash);
            return hash;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Parse(string hashString, out Hash128 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string Internal_Hash128ToString(uint d0, uint d1, uint d2, uint d3);
        public override bool Equals(object obj)
        {
            return ((obj is Hash128) && (this == ((Hash128) obj)));
        }

        public override int GetHashCode()
        {
            return (((this.m_u32_0.GetHashCode() ^ this.m_u32_1.GetHashCode()) ^ this.m_u32_2.GetHashCode()) ^ this.m_u32_3.GetHashCode());
        }

        public static bool operator ==(Hash128 hash1, Hash128 hash2)
        {
            return ((((hash1.m_u32_0 == hash2.m_u32_0) && (hash1.m_u32_1 == hash2.m_u32_1)) && (hash1.m_u32_2 == hash2.m_u32_2)) && (hash1.m_u32_3 == hash2.m_u32_3));
        }

        public static bool operator !=(Hash128 hash1, Hash128 hash2)
        {
            return !(hash1 == hash2);
        }
    }
}

