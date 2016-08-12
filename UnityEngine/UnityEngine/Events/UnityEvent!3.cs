﻿namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    [Serializable]
    public abstract class UnityEvent<T0, T1, T2> : UnityEventBase
    {
        private readonly object[] m_InvokeArray;

        [RequiredByNativeCode]
        public UnityEvent()
        {
            this.m_InvokeArray = new object[3];
        }

        public void AddListener(UnityAction<T0, T1, T2> call)
        {
            base.AddCall(UnityEvent<T0, T1, T2>.GetDelegate(call));
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2> call)
        {
            this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2> call, UnityEventCallState callState)
        {
            int persistentEventCount = base.GetPersistentEventCount();
            base.AddPersistentListener();
            this.RegisterPersistentListener(persistentEventCount, call);
            base.SetPersistentListenerState(persistentEventCount, callState);
        }

        protected override MethodInfo FindMethod_Impl(string name, object targetObj)
        {
            System.Type[] argumentTypes = new System.Type[] { typeof(T0), typeof(T1), typeof(T2) };
            return UnityEventBase.GetValidMethodInfo(targetObj, name, argumentTypes);
        }

        private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2> action)
        {
            return new InvokableCall<T0, T1, T2>(action);
        }

        internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new InvokableCall<T0, T1, T2>(target, theFunction);
        }

        public void Invoke(T0 arg0, T1 arg1, T2 arg2)
        {
            this.m_InvokeArray[0] = arg0;
            this.m_InvokeArray[1] = arg1;
            this.m_InvokeArray[2] = arg2;
            base.Invoke(this.m_InvokeArray);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2> call)
        {
            if (call == null)
            {
                Debug.LogWarning("Registering a Listener requires an action");
            }
            else
            {
                base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
            }
        }

        public void RemoveListener(UnityAction<T0, T1, T2> call)
        {
            base.RemoveListener(call.Target, call.GetMethodInfo());
        }
    }
}

