namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class VRSettings
    {
        public static void LoadDeviceByName(string deviceName)
        {
            string[] prioritizedDeviceNameList = new string[] { deviceName };
            LoadDeviceByName(prioritizedDeviceNameList);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadDeviceByName(string[] prioritizedDeviceNameList);

        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int eyeTextureHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int eyeTextureWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("loadedDevice is deprecated.  Use loadedDeviceName and LoadDeviceByName instead.")]
        public static VRDeviceType loadedDevice
        {
            get
            {
                VRDeviceType unknown = VRDeviceType.Unknown;
                try
                {
                    unknown = (VRDeviceType) ((int) Enum.Parse(typeof(VRDeviceType), loadedDeviceName, true));
                }
                catch (Exception)
                {
                }
                return unknown;
            }
            set
            {
                LoadDeviceByName(value.ToString());
            }
        }

        public static string loadedDeviceName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float renderScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float renderViewportScale
        {
            get
            {
                return renderViewportScaleInternal;
            }
            set
            {
                if ((value < 0f) || (value > 1f))
                {
                    throw new ArgumentOutOfRangeException("value", "Render viewport scale should be between 0 and 1.");
                }
                renderViewportScaleInternal = value;
            }
        }

        internal static float renderViewportScaleInternal { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool showDeviceView { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string[] supportedDevices { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

