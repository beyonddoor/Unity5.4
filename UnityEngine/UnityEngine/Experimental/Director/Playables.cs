namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class Playables
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void BeginIgnoreAllocationTracker();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern object CastToInternal(System.Type castType, IntPtr handle, int version);
        internal static bool CheckInputBounds(Playable playable, int inputIndex)
        {
            return playable.CheckInputBounds(inputIndex);
        }

        internal static bool CompareVersion(Playable lhs, Playable rhs)
        {
            return Playable.CompareVersion(lhs, rhs);
        }

        internal static bool ConnectInternal(ref Playable source, ref Playable target, int sourceOutputPort, int targetInputPort)
        {
            return INTERNAL_CALL_ConnectInternal(ref source, ref target, sourceOutputPort, targetInputPort);
        }

        internal static void DisconnectInternal(ref Playable target, int inputPort)
        {
            INTERNAL_CALL_DisconnectInternal(ref target, inputPort);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EndIgnoreAllocationTracker();
        internal static bool Equals(Playable isAPlayable, object mightBeAnythingOrNull)
        {
            return ((mightBeAnythingOrNull != null) && isAPlayable.Equals(mightBeAnythingOrNull));
        }

        internal static bool Equals(Playable lhs, Playable rhs)
        {
            return CompareVersion(lhs, rhs);
        }

        internal static double GetDurationValidated(Playable playable, System.Type typeofPlayable)
        {
            return playable.duration;
        }

        internal static int GetInputCountValidated(Playable playable, System.Type typeofPlayable)
        {
            return playable.inputCount;
        }

        internal static Playable GetInputValidated(Playable playable, int inputPort, System.Type typeofPlayable)
        {
            return playable.GetInput(inputPort);
        }

        internal static float GetInputWeightValidated(Playable playable, int index, System.Type typeofPlayable)
        {
            return playable.GetInputWeight(index);
        }

        internal static int GetOutputCountValidated(Playable playable, System.Type typeofPlayable)
        {
            return playable.outputCount;
        }

        internal static Playable GetOutputValidated(Playable playable, int outputPort, System.Type typeofPlayable)
        {
            return playable.GetOutput(outputPort);
        }

        internal static PlayState GetPlayStateValidated(Playable playable, System.Type typeofPlayable)
        {
            return playable.state;
        }

        internal static double GetTimeValidated(Playable playable, System.Type typeofPlayable)
        {
            return playable.time;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern System.Type GetTypeOfInternal(IntPtr handle, int version);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_ConnectInternal(ref Playable source, ref Playable target, int sourceOutputPort, int targetInputPort);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DisconnectInternal(ref Playable target, int inputPort);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_InternalDestroy(ref Playable playable);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetPlayableDeleteOnDisconnect(ref Playable target, bool value);
        internal static void InternalDestroy(ref Playable playable)
        {
            INTERNAL_CALL_InternalDestroy(ref playable);
        }

        internal static bool IsValid(Playable playable)
        {
            return playable.IsValid();
        }

        internal static void SetDurationValidated(Playable playable, double duration, System.Type typeofPlayable)
        {
            playable.duration = duration;
        }

        internal static void SetInputWeightValidated(Playable playable, int inputIndex, float weight, System.Type typeofPlayable)
        {
            playable.SetInputWeight(inputIndex, weight);
        }

        internal static void SetInputWeightValidated(Playable playable, Playable input, float weight, System.Type typeofPlayable)
        {
            playable.SetInputWeight(input, weight);
        }

        internal static void SetPlayableDeleteOnDisconnect(ref Playable target, bool value)
        {
            INTERNAL_CALL_SetPlayableDeleteOnDisconnect(ref target, value);
        }

        internal static void SetPlayStateValidated(Playable playable, PlayState playState, System.Type typeofPlayable)
        {
            playable.state = playState;
        }

        internal static void SetTimeValidated(Playable playable, double time, System.Type typeofPlayable)
        {
            playable.time = time;
        }
    }
}

