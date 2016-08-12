namespace UnityEngine.Analytics
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal class CustomEventData : IDisposable
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        private CustomEventData()
        {
        }

        public CustomEventData(string name)
        {
            this.InternalCreate(name);
        }

        ~CustomEventData()
        {
            this.InternalDestroy();
        }

        public void Dispose()
        {
            this.InternalDestroy();
            GC.SuppressFinalize(this);
        }

        public bool Add(string key, string value)
        {
            return this.AddString(key, value);
        }

        public bool Add(string key, bool value)
        {
            return this.AddBool(key, value);
        }

        public bool Add(string key, char value)
        {
            return this.AddChar(key, value);
        }

        public bool Add(string key, byte value)
        {
            return this.AddByte(key, value);
        }

        public bool Add(string key, sbyte value)
        {
            return this.AddSByte(key, value);
        }

        public bool Add(string key, short value)
        {
            return this.AddInt16(key, value);
        }

        public bool Add(string key, ushort value)
        {
            return this.AddUInt16(key, value);
        }

        public bool Add(string key, int value)
        {
            return this.AddInt32(key, value);
        }

        public bool Add(string key, uint value)
        {
            return this.AddUInt32(key, value);
        }

        public bool Add(string key, long value)
        {
            return this.AddInt64(key, value);
        }

        public bool Add(string key, ulong value)
        {
            return this.AddUInt64(key, value);
        }

        public bool Add(string key, float value)
        {
            return this.AddDouble(key, (double) Convert.ToDecimal(value));
        }

        public bool Add(string key, double value)
        {
            return this.AddDouble(key, value);
        }

        public bool Add(string key, decimal value)
        {
            return this.AddDouble(key, (double) Convert.ToDecimal(value));
        }

        public bool Add(IDictionary<string, object> eventData)
        {
            IEnumerator<KeyValuePair<string, object>> enumerator = eventData.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, object> current = enumerator.Current;
                    string key = current.Key;
                    object obj2 = current.Value;
                    if (obj2 != null)
                    {
                        System.Type type = obj2.GetType();
                        if (type != typeof(string))
                        {
                            if (type != typeof(char))
                            {
                                if (type != typeof(sbyte))
                                {
                                    if (type != typeof(byte))
                                    {
                                        if (type != typeof(short))
                                        {
                                            if (type != typeof(ushort))
                                            {
                                                if (type != typeof(int))
                                                {
                                                    if (type != typeof(uint))
                                                    {
                                                        if (type != typeof(long))
                                                        {
                                                            if (type != typeof(ulong))
                                                            {
                                                                if (type != typeof(bool))
                                                                {
                                                                    if (type != typeof(float))
                                                                    {
                                                                        if (type != typeof(double))
                                                                        {
                                                                            if (type != typeof(decimal))
                                                                            {
                                                                                if (!type.IsValueType)
                                                                                {
                                                                                    throw new ArgumentException(string.Format("Invalid type: {0} passed", type));
                                                                                }
                                                                                this.Add(key, obj2.ToString());
                                                                            }
                                                                            else
                                                                            {
                                                                                this.Add(key, (decimal) obj2);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            this.Add(key, (double) obj2);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        this.Add(key, (float) obj2);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    this.Add(key, (bool) obj2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                this.Add(key, (ulong) obj2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            this.Add(key, (long) obj2);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        this.Add(current.Key, (uint) obj2);
                                                    }
                                                }
                                                else
                                                {
                                                    this.Add(key, (int) obj2);
                                                }
                                            }
                                            else
                                            {
                                                this.Add(key, (ushort) obj2);
                                            }
                                        }
                                        else
                                        {
                                            this.Add(key, (short) obj2);
                                        }
                                    }
                                    else
                                    {
                                        this.Add(key, (byte) obj2);
                                    }
                                }
                                else
                                {
                                    this.Add(key, (sbyte) obj2);
                                }
                            }
                            else
                            {
                                this.Add(key, (char) obj2);
                            }
                            continue;
                        }
                        this.Add(key, (string) obj2);
                    }
                    else
                    {
                        this.Add(key, "null");
                        continue;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InternalCreate(string name);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        internal extern void InternalDestroy();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddString(string key, string value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddBool(string key, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddChar(string key, char value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddByte(string key, byte value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddSByte(string key, sbyte value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddInt16(string key, short value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddUInt16(string key, ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddInt32(string key, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddUInt32(string key, uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddInt64(string key, long value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddUInt64(string key, ulong value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool AddDouble(string key, double value);
    }
}

