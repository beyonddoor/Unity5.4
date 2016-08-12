namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public class Collider2D : Behaviour
    {
        [ExcludeFromDocs]
        public int Cast(Vector2 direction, RaycastHit2D[] results)
        {
            bool ignoreSiblingColliders = true;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_Cast(this, ref direction, results, positiveInfinity, ignoreSiblingColliders);
        }

        [ExcludeFromDocs]
        public int Cast(Vector2 direction, RaycastHit2D[] results, float distance)
        {
            bool ignoreSiblingColliders = true;
            return INTERNAL_CALL_Cast(this, ref direction, results, distance, ignoreSiblingColliders);
        }

        public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders)
        {
            return INTERNAL_CALL_Cast(this, ref direction, results, distance, ignoreSiblingColliders);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_Cast(Collider2D self, ref Vector2 direction, RaycastHit2D[] results, float distance, bool ignoreSiblingColliders);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_OverlapPoint(Collider2D self, ref Vector2 point);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_Raycast(Collider2D self, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_offset(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_offset(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsTouching(Collider2D collider);
        [ExcludeFromDocs]
        public bool IsTouchingLayers()
        {
            int layerMask = -1;
            return this.IsTouchingLayers(layerMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);
        public bool OverlapPoint(Vector2 point)
        {
            return INTERNAL_CALL_OverlapPoint(this, ref point);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -1;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -1;
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        public int Raycast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("Physics2D.AllLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, maxDepth);
        }

        public Rigidbody2D attachedRigidbody { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
        }

        public float density { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal ColliderErrorState2D errorState { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isTrigger { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector2 offset
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_offset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_offset(ref value);
            }
        }

        public int shapeCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public PhysicsMaterial2D sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool usedByEffector { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

