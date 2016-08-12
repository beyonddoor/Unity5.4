namespace UnityEngine.WSA
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Launcher
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InternalLaunchFileWithPicker(string fileExtension);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InternalLaunchUri(string uri, bool showWarning);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LaunchFile(Folder folder, string relativeFilePath, bool showWarning);
        public static void LaunchFileWithPicker(string fileExtension)
        {
            Process.Start("explorer.exe");
        }

        public static void LaunchUri(string uri, bool showWarning)
        {
            Process.Start(uri);
        }
    }
}

