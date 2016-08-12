namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AvatarBuilder
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Avatar BuildGenericAvatar(GameObject go, string rootMotionTransformName);
        public static Avatar BuildHumanAvatar(GameObject go, HumanDescription humanDescription)
        {
            if (go == null)
            {
                throw new NullReferenceException();
            }
            return BuildHumanAvatarMono(go, humanDescription);
        }

        private static Avatar BuildHumanAvatarMono(GameObject go, HumanDescription monoHumanDescription)
        {
            return INTERNAL_CALL_BuildHumanAvatarMono(go, ref monoHumanDescription);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Avatar INTERNAL_CALL_BuildHumanAvatarMono(GameObject go, ref HumanDescription monoHumanDescription);
    }
}

