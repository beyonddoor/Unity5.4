namespace UnityEngine.Apple.ReplayKit
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;

    public static class ReplayKit
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Discard();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Preview();
        [ExcludeFromDocs]
        public static bool StartRecording()
        {
            bool enableMicrophone = false;
            return StartRecording(enableMicrophone);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool StartRecording([DefaultValue("false")] bool enableMicrophone);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool StopRecording();

        public static bool APIAvailable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isRecording { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string lastError { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool recordingAvailable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

