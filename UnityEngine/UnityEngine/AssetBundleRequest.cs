namespace UnityEngine
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class AssetBundleRequest : AsyncOperation
    {
        public Object asset { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public Object[] allAssets { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

