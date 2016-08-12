﻿namespace SimpleJson
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [EditorBrowsable(EditorBrowsableState.Never), GeneratedCode("simple-json", "1.0.0")]
    internal class JsonObject : IDictionary<string, object>, IEnumerable, IEnumerable<KeyValuePair<string, object>>, ICollection<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _members;

        public JsonObject()
        {
            this._members = new Dictionary<string, object>();
        }

        public JsonObject(IEqualityComparer<string> comparer)
        {
            this._members = new Dictionary<string, object>(comparer);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this._members.Add(item.Key, item.Value);
        }

        public void Add(string key, object value)
        {
            this._members.Add(key, value);
        }

        public void Clear()
        {
            this._members.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return (this._members.ContainsKey(item.Key) && (this._members[item.Key] == item.Value));
        }

        public bool ContainsKey(string key)
        {
            return this._members.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            int count = this.Count;
            IEnumerator<KeyValuePair<string, object>> enumerator = this.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, object> current = enumerator.Current;
                    array[arrayIndex++] = current;
                    if (--count <= 0)
                    {
                        return;
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
        }

        internal static object GetAtIndex(IDictionary<string, object> obj, int index)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (index >= obj.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            int num = 0;
            IEnumerator<KeyValuePair<string, object>> enumerator = obj.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, object> current = enumerator.Current;
                    if (num++ == index)
                    {
                        return current.Value;
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
            return null;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this._members.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return this._members.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return this._members.Remove(item.Key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._members.GetEnumerator();
        }

        public override string ToString()
        {
            return SimpleJson.SimpleJson.SerializeObject(this);
        }

        public bool TryGetValue(string key, out object value)
        {
            return this._members.TryGetValue(key, out value);
        }

        public int Count
        {
            get
            {
                return this._members.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object this[int index]
        {
            get
            {
                return GetAtIndex(this._members, index);
            }
        }

        public object this[string key]
        {
            get
            {
                return this._members[key];
            }
            set
            {
                this._members[key] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this._members.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return this._members.Values;
            }
        }
    }
}

