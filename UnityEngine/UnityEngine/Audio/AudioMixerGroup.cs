﻿namespace UnityEngine.Audio
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AudioMixerGroup : UnityEngine.Object
    {
        internal AudioMixerGroup()
        {
        }

        public AudioMixer audioMixer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

