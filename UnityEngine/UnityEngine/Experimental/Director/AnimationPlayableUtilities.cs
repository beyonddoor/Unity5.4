namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class AnimationPlayableUtilities
    {
        internal static int AddInputValidated(AnimationPlayable target, Playable input, System.Type typeofTarget)
        {
            return target.AddInput(input);
        }

        internal static bool RemoveAllInputsValidated(AnimationPlayable target, System.Type typeofTarget)
        {
            return target.RemoveAllInputs();
        }

        internal static bool RemoveInputValidated(AnimationPlayable target, int index, System.Type typeofTarget)
        {
            return target.RemoveInput(index);
        }

        internal static bool RemoveInputValidated(AnimationPlayable target, Playable playable, System.Type typeofTarget)
        {
            return target.RemoveInput(playable);
        }

        internal static bool SetInputs(AnimationMixerPlayable playable, AnimationClip[] clips)
        {
            if (clips == null)
            {
                throw new NullReferenceException("Parameter clips was null. You need to pass in a valid array of clips.");
            }
            Playables.BeginIgnoreAllocationTracker();
            Playable[] sources = new Playable[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                sources[i] = (Playable) AnimationClipPlayable.Create(clips[i]);
                Playable target = sources[i];
                Playables.SetPlayableDeleteOnDisconnect(ref target, true);
            }
            Playables.EndIgnoreAllocationTracker();
            return SetInputsValidated((AnimationPlayable) playable, sources, typeof(AnimationMixerPlayable));
        }

        internal static bool SetInputsValidated(AnimationPlayable target, IEnumerable<Playable> sources, System.Type typeofTarget)
        {
            return target.SetInputs(sources);
        }

        internal static bool SetInputValidated(AnimationPlayable target, Playable source, int index, System.Type typeofTarget)
        {
            return target.SetInput(source, index);
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
    }
}

