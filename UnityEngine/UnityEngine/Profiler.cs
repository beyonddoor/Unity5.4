﻿namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public sealed class Profiler
    {
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("UNITY_EDITOR"), WrapperlessIcall]
        public static extern void AddFramesFromFile(string file);
        [Conditional("ENABLE_PROFILER")]
        public static void BeginSample(string name)
        {
            BeginSampleOnly(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), Conditional("ENABLE_PROFILER"), WrapperlessIcall]
        public static extern void BeginSample(string name, UnityEngine.Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void BeginSampleOnly(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Conditional("ENABLE_PROFILER")]
        public static extern void EndSample();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern uint GetMonoHeapSize();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern uint GetMonoUsedSize();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetRuntimeMemorySize(UnityEngine.Object o);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern uint GetTotalAllocatedMemory();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern uint GetTotalReservedMemory();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern uint GetTotalUnusedReservedMemory();

        public static bool enableBinaryLog { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string logFile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int maxNumberOfSamplesPerFrame { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool supported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static uint usedHeapSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

