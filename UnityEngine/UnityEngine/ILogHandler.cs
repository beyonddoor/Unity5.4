namespace UnityEngine
{
    using System;

    public interface ILogHandler
    {
        void LogException(Exception exception, UnityEngine.Object context);
        void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args);
    }
}

