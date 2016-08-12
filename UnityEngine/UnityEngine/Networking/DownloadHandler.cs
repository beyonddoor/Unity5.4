﻿namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    public class DownloadHandler : IDisposable
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        internal DownloadHandler()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalCreateBuffer();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalCreateScript();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalCreateTexture(bool readable);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalCreateAssetBundle(string url, uint crc);
        internal void InternalCreateAssetBundle(string url, Hash128 hash, uint crc)
        {
            INTERNAL_CALL_InternalCreateAssetBundle(this, url, ref hash, crc);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_InternalCreateAssetBundle(DownloadHandler self, string url, ref Hash128 hash, uint crc);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalCreateAudioClip(string url, AudioType audioType);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        private extern void InternalDestroy();
        ~DownloadHandler()
        {
            this.InternalDestroy();
        }

        public void Dispose()
        {
            this.InternalDestroy();
            GC.SuppressFinalize(this);
        }

        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public byte[] data
        {
            get
            {
                return this.GetData();
            }
        }
        public string text
        {
            get
            {
                return this.GetText();
            }
        }
        protected virtual byte[] GetData()
        {
            return null;
        }

        protected virtual string GetText()
        {
            byte[] data = this.GetData();
            if ((data != null) && (data.Length > 0))
            {
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
            return string.Empty;
        }

        [UsedByNativeCode]
        protected virtual bool ReceiveData(byte[] data, int dataLength)
        {
            return true;
        }

        [UsedByNativeCode]
        protected virtual void ReceiveContentLength(int contentLength)
        {
        }

        [UsedByNativeCode]
        protected virtual void CompleteContent()
        {
        }

        [UsedByNativeCode]
        protected virtual float GetProgress()
        {
            return 0f;
        }

        protected static T GetCheckedDownloader<T>(UnityWebRequest www) where T: DownloadHandler
        {
            if (www == null)
            {
                throw new NullReferenceException("Cannot get content from a null UnityWebRequest object");
            }
            if (!www.isDone)
            {
                throw new InvalidOperationException("Cannot get content from an unfinished UnityWebRequest object");
            }
            if (www.isError)
            {
                throw new InvalidOperationException(www.error);
            }
            return (T) www.downloadHandler;
        }
    }
}

