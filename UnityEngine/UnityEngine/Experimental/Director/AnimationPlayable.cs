namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimationPlayable
    {
        internal Playable handle;
        internal Playable node
        {
            get
            {
                return this.handle;
            }
        }
        public void Destroy()
        {
            this.node.Destroy();
        }

        public unsafe int AddInput(Playable input)
        {
            if (!Playable.Connect(input, *((Playable*) this), -1, -1))
            {
                throw new InvalidOperationException("AddInput Failed. Either the connected playable is incompatible or this AnimationPlayable type doesn't support adding inputs");
            }
            return (this.inputCount - 1);
        }

        public unsafe bool SetInput(Playable source, int index)
        {
            if (!this.node.CheckInputBounds(index))
            {
                return false;
            }
            if (this.GetInput(index).IsValid())
            {
                Playable.Disconnect(*((Playable*) this), index);
            }
            return Playable.Connect(source, *((Playable*) this), -1, index);
        }

        public unsafe bool SetInputs(IEnumerable<Playable> sources)
        {
            for (int i = 0; i < this.inputCount; i++)
            {
                Playable.Disconnect(*((Playable*) this), i);
            }
            bool flag = false;
            int targetInputPort = 0;
            IEnumerator<Playable> enumerator = sources.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Playable current = enumerator.Current;
                    if (targetInputPort < this.inputCount)
                    {
                        flag |= Playable.Connect(current, *((Playable*) this), -1, targetInputPort);
                    }
                    else
                    {
                        flag |= Playable.Connect(current, *((Playable*) this), -1, -1);
                    }
                    this.node.SetInputWeight(targetInputPort, 1f);
                    targetInputPort++;
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            for (int j = targetInputPort; j < this.inputCount; j++)
            {
                this.node.SetInputWeight(j, 0f);
            }
            return flag;
        }

        public unsafe bool RemoveInput(int index)
        {
            if (!Playables.CheckInputBounds(*((Playable*) this), index))
            {
                return false;
            }
            Playable.Disconnect(*((Playable*) this), index);
            return true;
        }

        public unsafe bool RemoveInput(Playable playable)
        {
            for (int i = 0; i < this.inputCount; i++)
            {
                if (this.GetInput(i) == playable)
                {
                    Playable.Disconnect(*((Playable*) this), i);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveAllInputs()
        {
            int inputCount = this.node.inputCount;
            for (int i = 0; i < inputCount; i++)
            {
                this.RemoveInput(i);
            }
            return true;
        }

        public static AnimationPlayable Null
        {
            get
            {
                AnimationPlayable playable = new AnimationPlayable();
                playable.handle.m_Version = 10;
                return playable;
            }
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

        public T CastTo<T>() where T: struct
        {
            return this.handle.CastTo<T>();
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
        public static bool operator ==(AnimationPlayable x, Playable y)
        {
            return Playables.Equals((Playable) x, y);
        }

        public static bool operator !=(AnimationPlayable x, Playable y)
        {
            return !Playables.Equals((Playable) x, y);
        }

        public static implicit operator Playable(AnimationPlayable b)
        {
            return b.node;
        }
    }
}

