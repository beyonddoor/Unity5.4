namespace UnityEngine
{
    using System;

    [Flags]
    public enum ComputeBufferType
    {
        Append = 2,
        Counter = 4,
        Default = 0,
        [Obsolete("Enum member DrawIndirect has been deprecated. Use IndirectArguments instead (UnityUpgradable) -> IndirectArguments", false)]
        DrawIndirect = 0x100,
        GPUMemory = 0x200,
        IndirectArguments = 0x100,
        Raw = 1
    }
}

