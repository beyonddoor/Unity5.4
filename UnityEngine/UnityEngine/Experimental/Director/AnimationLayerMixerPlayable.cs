namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    internal struct AnimationLayerMixerPlayable
    {
        internal AnimationPlayable handle;
        internal Playable node
        {
            get
            {
                return this.handle.node;
            }
        }
        public static AnimationLayerMixerPlayable Create()
        {
            AnimationLayerMixerPlayable that = new AnimationLayerMixerPlayable();
            InternalCreate(ref that);
            return that;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InternalCreate(ref AnimationLayerMixerPlayable that);
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

        public int inputCount
        {
            get
            {
                return Playables.GetInputCountValidated(*((Playable*) this), base.GetType());
            }
        }
        public unsafe Playable GetInput(int inputPort)
        {
            return Playables.GetInputValidated(*((Playable*) this), inputPort, base.GetType());
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

        public unsafe float GetInputWeight(int index)
        {
            return Playables.GetInputWeightValidated(*((Playable*) this), index, base.GetType());
        }

        public unsafe void SetInputWeight(int inputIndex, float weight)
        {
            Playables.SetInputWeightValidated(*((Playable*) this), inputIndex, weight, base.GetType());
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
        public T CastTo<T>() where T: struct
        {
            return this.handle.CastTo<T>();
        }

        public unsafe int AddInput(Playable input)
        {
            return AnimationPlayableUtilities.AddInputValidated(*((AnimationPlayable*) this), input, base.GetType());
        }

        public unsafe bool SetInput(Playable source, int index)
        {
            return AnimationPlayableUtilities.SetInputValidated(*((AnimationPlayable*) this), source, index, base.GetType());
        }

        public unsafe bool SetInputs(IEnumerable<Playable> sources)
        {
            return AnimationPlayableUtilities.SetInputsValidated(*((AnimationPlayable*) this), sources, base.GetType());
        }

        public unsafe bool RemoveInput(int index)
        {
            return AnimationPlayableUtilities.RemoveInputValidated(*((AnimationPlayable*) this), index, base.GetType());
        }

        public unsafe bool RemoveAllInputs()
        {
            return AnimationPlayableUtilities.RemoveAllInputsValidated(*((AnimationPlayable*) this), base.GetType());
        }

        public static bool operator ==(AnimationLayerMixerPlayable x, Playable y)
        {
            return Playables.Equals((Playable) x, y);
        }

        public static bool operator !=(AnimationLayerMixerPlayable x, Playable y)
        {
            return !Playables.Equals((Playable) x, y);
        }

        public static implicit operator Playable(AnimationLayerMixerPlayable b)
        {
            return b.node;
        }

        public static implicit operator AnimationPlayable(AnimationLayerMixerPlayable b)
        {
            return b.handle;
        }
    }
}

