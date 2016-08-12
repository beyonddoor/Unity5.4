namespace UnityEngine.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Application
    {
        public static  event WindowActivated windowActivated;

        public static  event WindowSizeChanged windowSizeChanged;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetAdvertisingIdentifier();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetAppArguments();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        internal static extern bool InternalTryInvokeOnAppThread(AppCallbackItem item, bool waitUntilDone);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        internal static extern bool InternalTryInvokeOnUIThread(AppCallbackItem item, bool waitUntilDone);
        public static void InvokeOnAppThread(AppCallbackItem item, bool waitUntilDone)
        {
            item();
        }

        public static void InvokeOnUIThread(AppCallbackItem item, bool waitUntilDone)
        {
            item();
        }

        internal static void InvokeWindowActivatedEvent(WindowActivationState state)
        {
            if (windowActivated != null)
            {
                windowActivated(state);
            }
        }

        internal static void InvokeWindowSizeChangedEvent(int width, int height)
        {
            if (windowSizeChanged != null)
            {
                windowSizeChanged(width, height);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        public static extern bool RunningOnAppThread();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        public static extern bool RunningOnUIThread();
        [Obsolete("TryInvokeOnAppThread is deprecated, use InvokeOnAppThread")]
        public static bool TryInvokeOnAppThread(AppCallbackItem item, bool waitUntilDone)
        {
            item();
            return true;
        }

        [Obsolete("TryInvokeOnUIThread is deprecated, use InvokeOnUIThread")]
        public static bool TryInvokeOnUIThread(AppCallbackItem item, bool waitUntilDone)
        {
            item();
            return true;
        }

        public static string advertisingIdentifier
        {
            get
            {
                string advertisingIdentifier = GetAdvertisingIdentifier();
                UnityEngine.Application.InvokeOnAdvertisingIdentifierCallback(advertisingIdentifier, true);
                return advertisingIdentifier;
            }
        }

        public static string arguments
        {
            get
            {
                return GetAppArguments();
            }
        }
    }
}

