namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;

    internal abstract class ResponseBase
    {
        protected ResponseBase()
        {
        }

        public abstract void Parse(object obj);
        public bool ParseJSONBool(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (!dictJsonObj.TryGetValue(name, out obj))
            {
                throw new FormatException(name + " not found in JSON dictionary");
            }
            return Convert.ToBoolean(obj);
        }

        public DateTime ParseJSONDateTime(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            throw new FormatException(name + " DateTime not yet supported");
        }

        public short ParseJSONInt16(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (!dictJsonObj.TryGetValue(name, out obj))
            {
                throw new FormatException(name + " not found in JSON dictionary");
            }
            return Convert.ToInt16(obj);
        }

        public int ParseJSONInt32(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (!dictJsonObj.TryGetValue(name, out obj))
            {
                throw new FormatException(name + " not found in JSON dictionary");
            }
            return Convert.ToInt32(obj);
        }

        public long ParseJSONInt64(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (!dictJsonObj.TryGetValue(name, out obj))
            {
                throw new FormatException(name + " not found in JSON dictionary");
            }
            return Convert.ToInt64(obj);
        }

        public List<T> ParseJSONList<T>(string name, object obj, IDictionary<string, object> dictJsonObj) where T: ResponseBase, new()
        {
            if (dictJsonObj.TryGetValue(name, out obj))
            {
                List<object> list = obj as List<object>;
                if (list != null)
                {
                    List<T> list2 = new List<T>();
                    foreach (IDictionary<string, object> dictionary in list)
                    {
                        T item = Activator.CreateInstance<T>();
                        item.Parse(dictionary);
                        list2.Add(item);
                    }
                    return list2;
                }
            }
            throw new FormatException(name + " not found in JSON dictionary");
        }

        public List<string> ParseJSONListOfStrings(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (dictJsonObj.TryGetValue(name, out obj))
            {
                List<object> list = obj as List<object>;
                if (list != null)
                {
                    List<string> list2 = new List<string>();
                    foreach (IDictionary<string, object> dictionary in list)
                    {
                        IEnumerator<KeyValuePair<string, object>> enumerator = dictionary.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                KeyValuePair<string, object> current = enumerator.Current;
                                string item = (string) current.Value;
                                list2.Add(item);
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
                    return list2;
                }
            }
            throw new FormatException(name + " not found in JSON dictionary");
        }

        public string ParseJSONString(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (!dictJsonObj.TryGetValue(name, out obj))
            {
                throw new FormatException(name + " not found in JSON dictionary");
            }
            return (obj as string);
        }

        public ushort ParseJSONUInt16(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (!dictJsonObj.TryGetValue(name, out obj))
            {
                throw new FormatException(name + " not found in JSON dictionary");
            }
            return Convert.ToUInt16(obj);
        }

        public uint ParseJSONUInt32(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (!dictJsonObj.TryGetValue(name, out obj))
            {
                throw new FormatException(name + " not found in JSON dictionary");
            }
            return Convert.ToUInt32(obj);
        }

        public ulong ParseJSONUInt64(string name, object obj, IDictionary<string, object> dictJsonObj)
        {
            if (!dictJsonObj.TryGetValue(name, out obj))
            {
                throw new FormatException(name + " not found in JSON dictionary");
            }
            return Convert.ToUInt64(obj);
        }
    }
}

