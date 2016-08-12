namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [AttributeUsage(AttributeTargets.Assembly), RequiredByNativeCode]
    public class AssemblyIsEditorAssembly : Attribute
    {
    }
}

