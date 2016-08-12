namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimatorControllerPlayable
    {
        internal AnimationPlayable handle;
        internal Playable node
        {
            get
            {
                return this.handle.node;
            }
        }
        public static AnimatorControllerPlayable Create(RuntimeAnimatorController controller)
        {
            AnimatorControllerPlayable that = new AnimatorControllerPlayable();
            InternalCreate(controller, ref that);
            return that;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InternalCreate(RuntimeAnimatorController controller, ref AnimatorControllerPlayable that);
        public void Destroy()
        {
            this.node.Destroy();
        }

        public RuntimeAnimatorController animatorController
        {
            get
            {
                return GetAnimatorControllerInternal(ref this);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RuntimeAnimatorController GetAnimatorControllerInternal(ref AnimatorControllerPlayable that);
        public float GetFloat(string name)
        {
            return GetFloatString(ref this, name);
        }

        public float GetFloat(int id)
        {
            return GetFloatID(ref this, id);
        }

        public void SetFloat(string name, float value)
        {
            SetFloatString(ref this, name, value);
        }

        public void SetFloat(int id, float value)
        {
            SetFloatID(ref this, id, value);
        }

        public bool GetBool(string name)
        {
            return GetBoolString(ref this, name);
        }

        public bool GetBool(int id)
        {
            return GetBoolID(ref this, id);
        }

        public void SetBool(string name, bool value)
        {
            SetBoolString(ref this, name, value);
        }

        public void SetBool(int id, bool value)
        {
            SetBoolID(ref this, id, value);
        }

        public int GetInteger(string name)
        {
            return GetIntegerString(ref this, name);
        }

        public int GetInteger(int id)
        {
            return GetIntegerID(ref this, id);
        }

        public void SetInteger(string name, int value)
        {
            SetIntegerString(ref this, name, value);
        }

        public void SetInteger(int id, int value)
        {
            SetIntegerID(ref this, id, value);
        }

        public void SetTrigger(string name)
        {
            SetTriggerString(ref this, name);
        }

        public void SetTrigger(int id)
        {
            SetTriggerID(ref this, id);
        }

        public void ResetTrigger(string name)
        {
            ResetTriggerString(ref this, name);
        }

        public void ResetTrigger(int id)
        {
            ResetTriggerID(ref this, id);
        }

        public bool IsParameterControlledByCurve(string name)
        {
            return IsParameterControlledByCurveString(ref this, name);
        }

        public bool IsParameterControlledByCurve(int id)
        {
            return IsParameterControlledByCurveID(ref this, id);
        }

        public int layerCount
        {
            get
            {
                return GetLayerCountInternal(ref this);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetLayerCountInternal(ref AnimatorControllerPlayable that);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetLayerNameInternal(ref AnimatorControllerPlayable that, int layerIndex);
        public string GetLayerName(int layerIndex)
        {
            return GetLayerNameInternal(ref this, layerIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetLayerIndexInternal(ref AnimatorControllerPlayable that, string layerName);
        public int GetLayerIndex(string layerName)
        {
            return GetLayerIndexInternal(ref this, layerName);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float GetLayerWeightInternal(ref AnimatorControllerPlayable that, int layerIndex);
        public float GetLayerWeight(int layerIndex)
        {
            return GetLayerWeightInternal(ref this, layerIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetLayerWeightInternal(ref AnimatorControllerPlayable that, int layerIndex, float weight);
        public void SetLayerWeight(int layerIndex, float weight)
        {
            SetLayerWeightInternal(ref this, layerIndex, weight);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AnimatorStateInfo GetCurrentAnimatorStateInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
        {
            return GetCurrentAnimatorStateInfoInternal(ref this, layerIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AnimatorStateInfo GetNextAnimatorStateInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex)
        {
            return GetNextAnimatorStateInfoInternal(ref this, layerIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AnimatorTransitionInfo GetAnimatorTransitionInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
        {
            return GetAnimatorTransitionInfoInternal(ref this, layerIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AnimatorClipInfo[] GetCurrentAnimatorClipInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex)
        {
            return GetCurrentAnimatorClipInfoInternal(ref this, layerIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AnimatorClipInfo[] GetNextAnimatorClipInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);
        public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex)
        {
            return GetNextAnimatorClipInfoInternal(ref this, layerIndex);
        }

        internal string ResolveHash(int hash)
        {
            return ResolveHashInternal(ref this, hash);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string ResolveHashInternal(ref AnimatorControllerPlayable that, int hash);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool IsInTransitionInternal(ref AnimatorControllerPlayable that, int layerIndex);
        public bool IsInTransition(int layerIndex)
        {
            return IsInTransitionInternal(ref this, layerIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetParameterCountInternal(ref AnimatorControllerPlayable that);
        public int parameterCount
        {
            get
            {
                return GetParameterCountInternal(ref this);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AnimatorControllerParameter[] GetParametersArrayInternal(ref AnimatorControllerPlayable that);
        public AnimatorControllerParameter GetParameter(int index)
        {
            AnimatorControllerParameter[] parametersArrayInternal = GetParametersArrayInternal(ref this);
            if ((index < 0) && (index >= parametersArrayInternal.Length))
            {
                throw new IndexOutOfRangeException("index");
            }
            return parametersArrayInternal[index];
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        private static extern int StringToHash(string name);
        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(string stateName, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(string stateName, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
        }

        public void CrossFadeInFixedTime(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
        {
            CrossFadeInFixedTimeInternal(ref this, StringToHash(stateName), transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
        }

        public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
        {
            CrossFadeInFixedTimeInternal(ref this, stateNameHash, transitionDuration, layer, fixedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime);
        [ExcludeFromDocs]
        private static void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, int layer)
        {
            float fixedTime = 0f;
            CrossFadeInFixedTimeInternal(ref that, stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        private static void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration)
        {
            float fixedTime = 0f;
            int layer = -1;
            CrossFadeInFixedTimeInternal(ref that, stateNameHash, transitionDuration, layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void CrossFade(string stateName, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.CrossFade(stateName, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(string stateName, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.CrossFade(stateName, transitionDuration, layer, negativeInfinity);
        }

        public void CrossFade(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            CrossFadeInternal(ref this, StringToHash(stateName), transitionDuration, layer, normalizedTime);
        }

        [ExcludeFromDocs]
        public void CrossFade(int stateNameHash, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.CrossFade(stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void CrossFade(int stateNameHash, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.CrossFade(stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        public void CrossFade(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            CrossFadeInternal(ref this, stateNameHash, transitionDuration, layer, normalizedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);
        [ExcludeFromDocs]
        private static void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            CrossFadeInternal(ref that, stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private static void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            CrossFadeInternal(ref that, stateNameHash, transitionDuration, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(string stateName, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInFixedTime(stateName, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(string stateName)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInFixedTime(stateName, layer, negativeInfinity);
        }

        public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
        {
            PlayInFixedTimeInternal(ref this, StringToHash(stateName), layer, fixedTime);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void PlayInFixedTime(int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
        }

        public void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
        {
            PlayInFixedTimeInternal(ref this, stateNameHash, layer, fixedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime);
        [ExcludeFromDocs]
        private static void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            PlayInFixedTimeInternal(ref that, stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private static void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            PlayInFixedTimeInternal(ref that, stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(string stateName, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.Play(stateName, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(string stateName)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.Play(stateName, layer, negativeInfinity);
        }

        public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            this.PlayInternal(ref this, StringToHash(stateName), layer, normalizedTime);
        }

        [ExcludeFromDocs]
        public void Play(int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.Play(stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        public void Play(int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.Play(stateNameHash, layer, negativeInfinity);
        }

        public void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
        {
            this.PlayInternal(ref this, stateNameHash, layer, normalizedTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);
        [ExcludeFromDocs]
        private void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash, int layer)
        {
            float negativeInfinity = float.NegativeInfinity;
            this.PlayInternal(ref that, stateNameHash, layer, negativeInfinity);
        }

        [ExcludeFromDocs]
        private void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash)
        {
            float negativeInfinity = float.NegativeInfinity;
            int layer = -1;
            this.PlayInternal(ref that, stateNameHash, layer, negativeInfinity);
        }

        public bool HasState(int layerIndex, int stateID)
        {
            return this.HasStateInternal(ref this, layerIndex, stateID);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool HasStateInternal(ref AnimatorControllerPlayable that, int layerIndex, int stateID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetFloatString(ref AnimatorControllerPlayable that, string name, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetFloatID(ref AnimatorControllerPlayable that, int id, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float GetFloatString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float GetFloatID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetBoolString(ref AnimatorControllerPlayable that, string name, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetBoolID(ref AnimatorControllerPlayable that, int id, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetBoolString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetBoolID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetIntegerString(ref AnimatorControllerPlayable that, string name, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetIntegerID(ref AnimatorControllerPlayable that, int id, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetIntegerString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetIntegerID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetTriggerString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetTriggerID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void ResetTriggerString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void ResetTriggerID(ref AnimatorControllerPlayable that, int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool IsParameterControlledByCurveString(ref AnimatorControllerPlayable that, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool IsParameterControlledByCurveID(ref AnimatorControllerPlayable that, int id);
        public PlayState state
        {
            get
            {
                return Playables.GetPlayStateValidated(*((Playable*) this), base.GetType());
            }
            set
            {
                Playables.SetPlayStateValidated(*((Playable*) this), value, base.GetType());
            }
        }
        public double time
        {
            get
            {
                return Playables.GetTimeValidated(*((Playable*) this), base.GetType());
            }
            set
            {
                Playables.SetTimeValidated(*((Playable*) this), value, base.GetType());
            }
        }
        public double duration
        {
            get
            {
                return Playables.GetDurationValidated(*((Playable*) this), base.GetType());
            }
            set
            {
                Playables.SetDurationValidated(*((Playable*) this), value, base.GetType());
            }
        }
        public unsafe bool IsValid()
        {
            return Playables.IsValid(*((Playable*) this));
        }

        public T CastTo<T>() where T: struct
        {
            return this.handle.CastTo<T>();
        }

        public override unsafe bool Equals(object p)
        {
            return Playables.Equals(*((Playable*) this), p);
        }

        public override int GetHashCode()
        {
            return this.node.GetHashCode();
        }

        public static implicit operator Playable(AnimatorControllerPlayable s)
        {
            return s.node;
        }

        public static implicit operator AnimationPlayable(AnimatorControllerPlayable s)
        {
            return s.handle;
        }

        public static bool operator ==(AnimatorControllerPlayable x, Playable y)
        {
            return Playables.Equals((Playable) x, y);
        }

        public static bool operator !=(AnimatorControllerPlayable x, Playable y)
        {
            return !Playables.Equals((Playable) x, y);
        }
    }
}

