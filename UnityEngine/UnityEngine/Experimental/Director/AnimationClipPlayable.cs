namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimationClipPlayable
    {
        internal AnimationPlayable handle;
        internal Playable node
        {
            get
            {
                return this.handle.node;
            }
        }
        public static AnimationClipPlayable Create(AnimationClip clip)
        {
            AnimationClipPlayable that = new AnimationClipPlayable();
            InternalCreate(clip, ref that);
            return that;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InternalCreate(AnimationClip clip, ref AnimationClipPlayable that);
        public void Destroy()
        {
            this.node.Destroy();
        }

        public override unsafe bool Equals(object p)
        {
            return Playables.Equals(*((Playable*) this), p);
        }

        public override int GetHashCode()
        {
            return this.node.GetHashCode();
        }

        public unsafe bool IsValid()
        {
            return Playables.IsValid(*((Playable*) this));
        }

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
        public int outputCount
        {
            get
            {
                return Playables.GetOutputCountValidated(*((Playable*) this), base.GetType());
            }
        }
        public unsafe Playable GetOutput(int outputPort)
        {
            return Playables.GetOutputValidated(*((Playable*) this), outputPort, base.GetType());
        }

        public T CastTo<T>() where T: struct
        {
            return this.handle.CastTo<T>();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AnimationClip GetAnimationClip(ref AnimationClipPlayable that);
        public AnimationClip clip
        {
            get
            {
                return GetAnimationClip(ref this);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float GetSpeed(ref AnimationClipPlayable that);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetSpeed(ref AnimationClipPlayable that, float value);
        public float speed
        {
            get
            {
                return GetSpeed(ref this);
            }
            set
            {
                SetSpeed(ref this, value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetApplyFootIK(ref AnimationClipPlayable that);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetApplyFootIK(ref AnimationClipPlayable that, bool value);
        public bool applyFootIK
        {
            get
            {
                return GetApplyFootIK(ref this);
            }
            set
            {
                SetApplyFootIK(ref this, value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetRemoveStartOffset(ref AnimationClipPlayable that);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetRemoveStartOffset(ref AnimationClipPlayable that, bool value);
        internal bool removeStartOffset
        {
            get
            {
                return GetRemoveStartOffset(ref this);
            }
            set
            {
                SetRemoveStartOffset(ref this, value);
            }
        }
        public static bool operator ==(AnimationClipPlayable x, Playable y)
        {
            return Playables.Equals((Playable) x, y);
        }

        public static bool operator !=(AnimationClipPlayable x, Playable y)
        {
            return !Playables.Equals((Playable) x, y);
        }

        public static implicit operator Playable(AnimationClipPlayable b)
        {
            return b.node;
        }

        public static implicit operator AnimationPlayable(AnimationClipPlayable b)
        {
            return b.handle;
        }
    }
}

