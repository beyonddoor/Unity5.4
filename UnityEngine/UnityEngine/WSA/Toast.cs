namespace UnityEngine.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Toast
    {
        private int m_ToastId;

        private Toast(int id)
        {
            this.m_ToastId = id;
        }

        public static Toast Create(string xml)
        {
            int id = CreateToastXml(xml);
            if (id < 0)
            {
                return null;
            }
            return new Toast(id);
        }

        public static Toast Create(string image, string text)
        {
            int id = CreateToastImageAndText(image, text);
            if (id < 0)
            {
                return null;
            }
            return new Toast(id);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int CreateToastImageAndText(string image, string text);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int CreateToastXml(string xml);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetActivated(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetArguments(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetDismissed(int id, bool byUser);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetTemplate(ToastTemplate templ);
        public void Hide()
        {
            Hide(this.m_ToastId);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Hide(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetArguments(int id, string args);
        public void Show()
        {
            Show(this.m_ToastId);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Show(int id);

        public bool activated
        {
            get
            {
                return GetActivated(this.m_ToastId);
            }
        }

        public string arguments
        {
            get
            {
                return GetArguments(this.m_ToastId);
            }
            set
            {
                SetArguments(this.m_ToastId, value);
            }
        }

        public bool dismissed
        {
            get
            {
                return GetDismissed(this.m_ToastId, false);
            }
        }

        public bool dismissedByUser
        {
            get
            {
                return GetDismissed(this.m_ToastId, true);
            }
        }
    }
}

