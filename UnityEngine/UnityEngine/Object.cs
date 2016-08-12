namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class Object
    {
        private IntPtr m_CachedPtr;
        private int m_InstanceID;
        private string m_UnityRuntimeErrorString;
        internal static int OffsetOfInstanceIDInCPlusPlusObject = -1;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern UnityEngine.Object Internal_CloneSingle(UnityEngine.Object data);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        private static extern UnityEngine.Object Internal_CloneSingleWithParent(UnityEngine.Object data, Transform parent, bool worldPositionStays);
        [ThreadAndSerializationSafe]
        private static UnityEngine.Object Internal_InstantiateSingle(UnityEngine.Object data, Vector3 pos, Quaternion rot)
        {
            return INTERNAL_CALL_Internal_InstantiateSingle(data, ref pos, ref rot);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern UnityEngine.Object INTERNAL_CALL_Internal_InstantiateSingle(UnityEngine.Object data, ref Vector3 pos, ref Quaternion rot);
        [ThreadAndSerializationSafe]
        private static UnityEngine.Object Internal_InstantiateSingleWithParent(UnityEngine.Object data, Transform parent, Vector3 pos, Quaternion rot)
        {
            return INTERNAL_CALL_Internal_InstantiateSingleWithParent(data, parent, ref pos, ref rot);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern UnityEngine.Object INTERNAL_CALL_Internal_InstantiateSingleWithParent(UnityEngine.Object data, Transform parent, ref Vector3 pos, ref Quaternion rot);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        private static extern int GetOffsetOfInstanceIDInCPlusPlusObject();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private extern void EnsureRunningOnMainThread();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Destroy(UnityEngine.Object obj, [DefaultValue("0.0F")] float t);
        [ExcludeFromDocs]
        public static void Destroy(UnityEngine.Object obj)
        {
            float t = 0f;
            Destroy(obj, t);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DestroyImmediate(UnityEngine.Object obj, [DefaultValue("false")] bool allowDestroyingAssets);
        [ExcludeFromDocs]
        public static void DestroyImmediate(UnityEngine.Object obj)
        {
            bool allowDestroyingAssets = false;
            DestroyImmediate(obj, allowDestroyingAssets);
        }

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument), WrapperlessIcall]
        public static extern UnityEngine.Object[] FindObjectsOfType(System.Type type);
        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DontDestroyOnLoad(UnityEngine.Object target);
        public HideFlags hideFlags { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DestroyObject(UnityEngine.Object obj, [DefaultValue("0.0F")] float t);
        [ExcludeFromDocs]
        public static void DestroyObject(UnityEngine.Object obj)
        {
            float t = 0f;
            DestroyObject(obj, t);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("use Object.FindObjectsOfType instead.")]
        public static extern UnityEngine.Object[] FindSceneObjectsOfType(System.Type type);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use Resources.FindObjectsOfTypeAll instead."), WrapperlessIcall]
        public static extern UnityEngine.Object[] FindObjectsOfTypeIncludingAssets(System.Type type);
        [Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
        public static UnityEngine.Object[] FindObjectsOfTypeAll(System.Type type)
        {
            return Resources.FindObjectsOfTypeAll(type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public override extern string ToString();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        internal static extern bool DoesObjectWithInstanceIDExist(int instanceID);
        [SecuritySafeCritical]
        public int GetInstanceID()
        {
            this.EnsureRunningOnMainThread();
            return this.m_InstanceID;
        }

        public override int GetHashCode()
        {
            return this.m_InstanceID;
        }

        public override bool Equals(object o)
        {
            return CompareBaseObjects(this, o as UnityEngine.Object);
        }

        private static bool CompareBaseObjects(UnityEngine.Object lhs, UnityEngine.Object rhs)
        {
            bool flag = lhs == null;
            bool flag2 = rhs == null;
            if (flag2 && flag)
            {
                return true;
            }
            if (flag2)
            {
                return !IsNativeObjectAlive(lhs);
            }
            if (flag)
            {
                return !IsNativeObjectAlive(rhs);
            }
            return (lhs.m_InstanceID == rhs.m_InstanceID);
        }

        private static bool IsNativeObjectAlive(UnityEngine.Object o)
        {
            return ((o.GetCachedPtr() != IntPtr.Zero) || ((!(o is MonoBehaviour) && !(o is ScriptableObject)) && DoesObjectWithInstanceIDExist(o.GetInstanceID())));
        }

        private IntPtr GetCachedPtr()
        {
            return this.m_CachedPtr;
        }

        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation)
        {
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return Internal_InstantiateSingle(original, position, rotation);
        }

        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (parent == null)
            {
                return Internal_InstantiateSingle(original, position, rotation);
            }
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return Internal_InstantiateSingleWithParent(original, parent, position, rotation);
        }

        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original)
        {
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return Internal_CloneSingle(original);
        }

        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent)
        {
            return Instantiate(original, parent, true);
        }

        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent, bool worldPositionStays)
        {
            if (parent == null)
            {
                return Internal_CloneSingle(original);
            }
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return Internal_CloneSingleWithParent(original, parent, worldPositionStays);
        }

        public static T Instantiate<T>(T original) where T: UnityEngine.Object
        {
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return (T) Internal_CloneSingle(original);
        }

        private static void CheckNullArgument(object arg, string message)
        {
            if (arg == null)
            {
                throw new ArgumentException(message);
            }
        }

        public static T[] FindObjectsOfType<T>() where T: UnityEngine.Object
        {
            return Resources.ConvertObjects<T>(FindObjectsOfType(typeof(T)));
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static UnityEngine.Object FindObjectOfType(System.Type type)
        {
            UnityEngine.Object[] objArray = FindObjectsOfType(type);
            if (objArray.Length > 0)
            {
                return objArray[0];
            }
            return null;
        }

        public static T FindObjectOfType<T>() where T: UnityEngine.Object
        {
            return (T) FindObjectOfType(typeof(T));
        }

        public static implicit operator bool(UnityEngine.Object exists)
        {
            return !CompareBaseObjects(exists, null);
        }

        public static bool operator ==(UnityEngine.Object x, UnityEngine.Object y)
        {
            return CompareBaseObjects(x, y);
        }

        public static bool operator !=(UnityEngine.Object x, UnityEngine.Object y)
        {
            return !CompareBaseObjects(x, y);
        }
    }
}

