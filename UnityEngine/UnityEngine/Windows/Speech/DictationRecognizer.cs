namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    public sealed class DictationRecognizer : IDisposable
    {
        private IntPtr m_Recognizer;

        public event DictationCompletedDelegate DictationComplete;

        public event DictationErrorHandler DictationError;

        public event DictationHypothesisDelegate DictationHypothesis;

        public event DictationResultDelegate DictationResult;

        public DictationRecognizer() : this(ConfidenceLevel.Medium, DictationTopicConstraint.Dictation)
        {
        }

        public DictationRecognizer(ConfidenceLevel confidenceLevel) : this(confidenceLevel, DictationTopicConstraint.Dictation)
        {
        }

        public DictationRecognizer(DictationTopicConstraint topic) : this(ConfidenceLevel.Medium, topic)
        {
        }

        public DictationRecognizer(ConfidenceLevel minimumConfidence, DictationTopicConstraint topic)
        {
            this.m_Recognizer = this.Create(minimumConfidence, topic);
        }

        private IntPtr Create(ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint)
        {
            IntPtr ptr;
            INTERNAL_CALL_Create(this, minimumConfidence, topicConstraint, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Destroy(IntPtr self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void DestroyThreaded(IntPtr self);
        [RequiredByNativeCode]
        private void DictationRecognizer_InvokeCompletedEvent(DictationCompletionCause cause)
        {
            DictationCompletedDelegate dictationComplete = this.DictationComplete;
            if (dictationComplete != null)
            {
                dictationComplete(cause);
            }
        }

        [RequiredByNativeCode]
        private void DictationRecognizer_InvokeErrorEvent(string error, int hresult)
        {
            DictationErrorHandler dictationError = this.DictationError;
            if (dictationError != null)
            {
                dictationError(error, hresult);
            }
        }

        [RequiredByNativeCode]
        private void DictationRecognizer_InvokeHypothesisGeneratedEvent(string keyword)
        {
            DictationHypothesisDelegate dictationHypothesis = this.DictationHypothesis;
            if (dictationHypothesis != null)
            {
                dictationHypothesis(keyword);
            }
        }

        [RequiredByNativeCode]
        private void DictationRecognizer_InvokeResultGeneratedEvent(string keyword, ConfidenceLevel minimumConfidence)
        {
            DictationResultDelegate dictationResult = this.DictationResult;
            if (dictationResult != null)
            {
                dictationResult(keyword, minimumConfidence);
            }
        }

        public void Dispose()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                Destroy(this.m_Recognizer);
                this.m_Recognizer = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }

        ~DictationRecognizer()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                DestroyThreaded(this.m_Recognizer);
                this.m_Recognizer = IntPtr.Zero;
                GC.SuppressFinalize(this);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float GetAutoSilenceTimeoutSeconds(IntPtr self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float GetInitialSilenceTimeoutSeconds(IntPtr self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern SpeechSystemStatus GetStatus(IntPtr self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Create(DictationRecognizer self, ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetAutoSilenceTimeoutSeconds(IntPtr self, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetInitialSilenceTimeoutSeconds(IntPtr self, float value);
        public void Start()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                Start(this.m_Recognizer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Start(IntPtr self);
        public void Stop()
        {
            if (this.m_Recognizer != IntPtr.Zero)
            {
                Stop(this.m_Recognizer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Stop(IntPtr self);

        public float AutoSilenceTimeoutSeconds
        {
            get
            {
                if (this.m_Recognizer == IntPtr.Zero)
                {
                    return 0f;
                }
                return GetAutoSilenceTimeoutSeconds(this.m_Recognizer);
            }
            set
            {
                if (this.m_Recognizer != IntPtr.Zero)
                {
                    SetAutoSilenceTimeoutSeconds(this.m_Recognizer, value);
                }
            }
        }

        public float InitialSilenceTimeoutSeconds
        {
            get
            {
                if (this.m_Recognizer == IntPtr.Zero)
                {
                    return 0f;
                }
                return GetInitialSilenceTimeoutSeconds(this.m_Recognizer);
            }
            set
            {
                if (this.m_Recognizer != IntPtr.Zero)
                {
                    SetInitialSilenceTimeoutSeconds(this.m_Recognizer, value);
                }
            }
        }

        public SpeechSystemStatus Status
        {
            get
            {
                return (!(this.m_Recognizer != IntPtr.Zero) ? SpeechSystemStatus.Stopped : GetStatus(this.m_Recognizer));
            }
        }

        public delegate void DictationCompletedDelegate(DictationCompletionCause cause);

        public delegate void DictationErrorHandler(string error, int hresult);

        public delegate void DictationHypothesisDelegate(string text);

        public delegate void DictationResultDelegate(string text, ConfidenceLevel confidence);
    }
}

