namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class ParticleSystemExtensionsImpl
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetCollisionEvents(ParticleSystem ps, GameObject go, object collisionEvents);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetCollisionEventsDeprecated(ParticleSystem ps, GameObject go, ParticleCollisionEvent[] collisionEvents);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetSafeCollisionEventSize(ParticleSystem ps);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetSafeTriggerParticlesSize(ParticleSystem ps, int type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetTriggerParticles(ParticleSystem ps, int type, object particles);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetTriggerParticles(ParticleSystem ps, int type, object particles, int offset, int count);
    }
}

