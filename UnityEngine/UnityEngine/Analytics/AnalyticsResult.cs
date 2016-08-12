namespace UnityEngine.Analytics
{
    using System;

    public enum AnalyticsResult
    {
        Ok,
        NotInitialized,
        AnalyticsDisabled,
        TooManyItems,
        SizeLimitReached,
        TooManyRequests,
        InvalidData,
        UnsupportedPlatform
    }
}

