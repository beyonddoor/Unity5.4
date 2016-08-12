namespace UnityEngine.Scripting
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, Inherited=false)]
    internal class UsedByNativeCodeAttribute : Attribute
    {
        public UsedByNativeCodeAttribute()
        {
        }

        public UsedByNativeCodeAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}

