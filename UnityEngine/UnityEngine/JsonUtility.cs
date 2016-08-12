namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public static class JsonUtility
    {
        public static T FromJson<T>(string json)
        {
            return (T) FromJson(json, typeof(T));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        public static extern object FromJson(string json, System.Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        public static extern void FromJsonOverwrite(string json, object objectToOverwrite);
        public static string ToJson(object obj)
        {
            return ToJson(obj, false);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        public static extern string ToJson(object obj, bool prettyPrint);
    }
}

