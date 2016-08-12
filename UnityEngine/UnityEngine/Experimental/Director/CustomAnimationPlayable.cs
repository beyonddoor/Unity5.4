namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public class CustomAnimationPlayable : ScriptPlayable
    {
        internal AnimationPlayable handle;

        public CustomAnimationPlayable()
        {
            if (!this.handle.IsValid())
            {
                string str = base.GetType().ToString();
                string[] textArray1 = new string[] { str, " must be instantiated using the Playable.Create<", str, "> method instead of new ", str, "." };
                throw new InvalidOperationException(string.Concat(textArray1));
            }
        }

        public int AddInput(Playable input)
        {
            return AnimationPlayableUtilities.AddInputValidated((AnimationPlayable) this, input, base.GetType());
        }

        public T CastTo<T>() where T: struct
        {
            return this.handle.CastTo<T>();
        }

        public void Destroy()
        {
            this.node.Destroy();
        }

        public Playable GetInput(int inputPort)
        {
            return Playables.GetInputValidated((Playable) this, inputPort, base.GetType());
        }

        public float GetInputWeight(int index)
        {
            return Playables.GetInputWeightValidated((Playable) this, index, base.GetType());
        }

        public Playable GetOutput(int outputPort)
        {
            return Playables.GetOutputValidated((Playable) this, outputPort, base.GetType());
        }

        public virtual void OnSetPlayState(PlayState newState)
        {
        }

        public virtual void OnSetTime(float localTime)
        {
        }

        public static implicit operator AnimationPlayable(CustomAnimationPlayable s)
        {
            return s.handle;
        }

        public static implicit operator Playable(CustomAnimationPlayable s)
        {
            return new Playable { m_Handle = s.node.m_Handle, m_Version = s.node.m_Version };
        }

        public virtual void PrepareFrame(FrameData info)
        {
        }

        public bool RemoveAllInputs()
        {
            return AnimationPlayableUtilities.RemoveAllInputsValidated((AnimationPlayable) this, base.GetType());
        }

        public bool RemoveInput(int index)
        {
            return AnimationPlayableUtilities.RemoveInputValidated((AnimationPlayable) this, index, base.GetType());
        }

        internal void SetHandle(int version, IntPtr playableHandle)
        {
            this.handle.handle.m_Handle = playableHandle;
            this.handle.handle.m_Version = version;
        }

        public bool SetInput(Playable source, int index)
        {
            return AnimationPlayableUtilities.SetInputValidated((AnimationPlayable) this, source, index, base.GetType());
        }

        public bool SetInputs(IEnumerable<Playable> sources)
        {
            return AnimationPlayableUtilities.SetInputsValidated((AnimationPlayable) this, sources, base.GetType());
        }

        public void SetInputWeight(int inputIndex, float weight)
        {
            Playables.SetInputWeightValidated((Playable) this, inputIndex, weight, base.GetType());
        }

        public double duration
        {
            get
            {
                return Playables.GetDurationValidated((Playable) this, base.GetType());
            }
            set
            {
                Playables.SetDurationValidated((Playable) this, value, base.GetType());
            }
        }

        public int inputCount
        {
            get
            {
                return Playables.GetInputCountValidated((Playable) this, base.GetType());
            }
        }

        internal Playable node
        {
            get
            {
                return (Playable) this.handle;
            }
        }

        public int outputCount
        {
            get
            {
                return Playables.GetOutputCountValidated((Playable) this, base.GetType());
            }
        }

        public PlayState state
        {
            get
            {
                return Playables.GetPlayStateValidated((Playable) this, base.GetType());
            }
            set
            {
                Playables.SetPlayStateValidated((Playable) this, value, base.GetType());
            }
        }

        public double time
        {
            get
            {
                return Playables.GetTimeValidated((Playable) this, base.GetType());
            }
            set
            {
                Playables.SetTimeValidated((Playable) this, value, base.GetType());
            }
        }
    }
}

