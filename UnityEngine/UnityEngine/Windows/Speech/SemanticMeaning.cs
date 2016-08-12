namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SemanticMeaning
    {
        public string key;
        public string[] values;
    }
}

