namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Ping
    {
        internal IntPtr m_Ptr;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Ping(string address);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        public extern void DestroyPing();
        ~Ping()
        {
            this.DestroyPing();
        }

        public string ip { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int time { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

