namespace JetBrains.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [BaseTypeRequired(typeof(Attribute)), AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public sealed class BaseTypeRequiredAttribute : Attribute
    {
        public BaseTypeRequiredAttribute([NotNull] Type baseType)
        {
            this.BaseType = baseType;
        }

        [NotNull]
        public Type BaseType { get; private set; }
    }
}

