﻿namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [RequiredByNativeCode, Obsolete("Use SerializeField on the private variables that you want to be serialized instead")]
    public sealed class SerializePrivateVariables : Attribute
    {
    }
}

