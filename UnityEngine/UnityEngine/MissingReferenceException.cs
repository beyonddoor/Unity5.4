﻿namespace UnityEngine
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class MissingReferenceException : SystemException
    {
        private const int Result = -2147467261;
        private string unityStackTrace;

        public MissingReferenceException() : base("A Unity Runtime error occurred!")
        {
            base.HResult = -2147467261;
        }

        public MissingReferenceException(string message) : base(message)
        {
            base.HResult = -2147467261;
        }

        protected MissingReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MissingReferenceException(string message, Exception innerException) : base(message, innerException)
        {
            base.HResult = -2147467261;
        }
    }
}

