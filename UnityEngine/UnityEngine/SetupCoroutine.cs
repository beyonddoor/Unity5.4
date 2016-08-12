namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    internal class SetupCoroutine
    {
        [RequiredByNativeCode]
        public static object InvokeMember(object behaviour, string name, object variable)
        {
            object[] args = null;
            if (variable != null)
            {
                args = new object[] { variable };
            }
            return behaviour.GetType().InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, behaviour, args, null, null, null);
        }

        [RequiredByNativeCode, SecuritySafeCritical]
        public static void InvokeMoveNext(IEnumerator enumerator, IntPtr returnValueAddress)
        {
            if (returnValueAddress == IntPtr.Zero)
            {
                throw new ArgumentException("Return value address cannot be 0.", "returnValueAddress");
            }
            returnValueAddress[0] = (IntPtr) enumerator.MoveNext();
        }

        public static object InvokeStatic(System.Type klass, string name, object variable)
        {
            object[] args = null;
            if (variable != null)
            {
                args = new object[] { variable };
            }
            return klass.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, null, args, null, null, null);
        }
    }
}

