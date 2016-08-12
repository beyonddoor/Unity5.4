namespace UnityEngine.Networking.Match
{
    using System;

    internal interface IResponse
    {
        void SetFailure(string info);
        void SetSuccess();
    }
}

