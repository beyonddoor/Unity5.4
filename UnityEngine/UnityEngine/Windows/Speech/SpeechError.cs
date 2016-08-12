namespace UnityEngine.Windows.Speech
{
    using System;

    public enum SpeechError
    {
        NoError,
        TopicLanguageNotSupported,
        GrammarLanguageMismatch,
        GrammarCompilationFailure,
        AudioQualityFailure,
        PauseLimitExceeded,
        TimeoutExceeded,
        NetworkFailure,
        MicrophoneUnavailable,
        UnknownError
    }
}

