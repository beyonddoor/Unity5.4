namespace UnityEngine
{
    using System;

    [Flags]
    public enum DepthTextureMode
    {
        Depth = 1,
        DepthNormals = 2,
        MotionVectors = 4,
        None = 0
    }
}

