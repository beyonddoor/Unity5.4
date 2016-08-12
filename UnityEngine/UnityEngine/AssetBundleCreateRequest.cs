namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class AssetBundleCreateRequest : AsyncOperation
    {
        public AssetBundle assetBundle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal CompatibilityCheck compatibilityChecks { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Flags]
        internal enum CompatibilityCheck
        {
            All = 7,
            ClassVersion = 4,
            None = 0,
            RuntimeVersion = 2,
            TypeTree = 1
        }
    }
}

