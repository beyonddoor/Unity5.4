namespace UnityEngine.VR
{
    using System;

    [Obsolete("VRDeviceType is deprecated. Use VRSettings.supportedDevices instead.")]
    public enum VRDeviceType
    {
        [Obsolete("Enum member VRDeviceType.Morpheus has been deprecated. Use VRDeviceType.PlayStationVR instead (UnityUpgradable) -> PlayStationVR", true)]
        Morpheus = -1,
        None = 0,
        Oculus = 3,
        PlayStationVR = 4,
        Split = 2,
        Stereo = 1,
        Unknown = 5
    }
}

