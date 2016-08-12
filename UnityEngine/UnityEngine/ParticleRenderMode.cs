namespace UnityEngine
{
    using System;

    [Obsolete("This is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false)]
    public enum ParticleRenderMode
    {
        Billboard = 0,
        HorizontalBillboard = 4,
        SortedBillboard = 2,
        Stretch = 3,
        VerticalBillboard = 5
    }
}

