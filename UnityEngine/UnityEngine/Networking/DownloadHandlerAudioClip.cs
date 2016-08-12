namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class DownloadHandlerAudioClip : DownloadHandler
    {
        public DownloadHandlerAudioClip(string url, AudioType audioType)
        {
            base.InternalCreateAudioClip(url, audioType);
        }

        protected override byte[] GetData()
        {
            return this.InternalGetData();
        }

        protected override string GetText()
        {
            throw new NotSupportedException("String access is not supported for audio clips");
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern byte[] InternalGetData();
        public AudioClip audioClip { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public static AudioClip GetContent(UnityWebRequest www)
        {
            return DownloadHandler.GetCheckedDownloader<DownloadHandlerAudioClip>(www).audioClip;
        }
    }
}

