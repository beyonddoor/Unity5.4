namespace UnityEngine.Scripting
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, Inherited=false)]
    internal class RequiredByNativeCodeAttribute : Attribute
    {
        public RequiredByNativeCodeAttribute()
        {
        }

        public RequiredByNativeCodeAttribute(bool optional)
        {
            this.Optional = optional;
        }

        public RequiredByNativeCodeAttribute(string name)
        {
            this.Name = name;
        }

        public RequiredByNativeCodeAttribute(string name, bool optional)
        {
            this.Name = name;
            this.Optional = optional;
        }

        public string Name { get; set; }

        public bool Optional { get; set; }
    }
}

