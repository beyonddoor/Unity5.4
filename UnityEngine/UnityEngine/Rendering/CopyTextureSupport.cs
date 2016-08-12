namespace UnityEngine.Rendering
{
    using System;

    [Flags]
    public enum CopyTextureSupport
    {
        Basic = 1,
        Copy3D = 2,
        DifferentTypes = 4,
        None = 0,
        RTToTexture = 0x10,
        TextureToRT = 8
    }
}

