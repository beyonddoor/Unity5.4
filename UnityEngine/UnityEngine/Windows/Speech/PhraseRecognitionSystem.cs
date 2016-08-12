namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    public static class PhraseRecognitionSystem
    {
        public static  event ErrorDelegate OnError;

        public static  event StatusDelegate OnStatusChanged;

        [RequiredByNativeCode]
        private static void PhraseRecognitionSystem_InvokeErrorEvent(SpeechError errorCode)
        {
            ErrorDelegate onError = OnError;
            if (onError != null)
            {
                onError(errorCode);
            }
        }

        [RequiredByNativeCode]
        private static void PhraseRecognitionSystem_InvokeStatusChangedEvent(SpeechSystemStatus status)
        {
            StatusDelegate onStatusChanged = OnStatusChanged;
            if (onStatusChanged != null)
            {
                onStatusChanged(status);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Restart();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Shutdown();

        [ThreadAndSerializationSafe]
        public static bool isSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static SpeechSystemStatus Status { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public delegate void ErrorDelegate(SpeechError errorCode);

        public delegate void StatusDelegate(SpeechSystemStatus status);
    }
}

