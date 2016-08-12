namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true), RequiredByNativeCode, Obsolete("NetworkView RPC functions are deprecated. Refer to the new Multiplayer Networking system.")]
    public sealed class RPC : Attribute
    {
    }
}

