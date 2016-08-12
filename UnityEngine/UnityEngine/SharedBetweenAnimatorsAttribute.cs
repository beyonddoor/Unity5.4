namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [RequiredByNativeCode, AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class SharedBetweenAnimatorsAttribute : Attribute
    {
    }
}

