namespace UnityEngine.Windows.Speech
{
    using System;

    public enum DictationCompletionCause
    {
        Complete,
        AudioQualityFailure,
        Canceled,
        TimeoutExceeded,
        PauseLimitExceeded,
        NetworkFailure,
        MicrophoneUnavailable,
        UnknownError
    }
}

