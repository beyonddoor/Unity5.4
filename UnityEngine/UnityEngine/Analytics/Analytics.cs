namespace UnityEngine.Analytics
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Analytics
    {
        private static UnityAnalyticsHandler s_UnityAnalyticsHandler;

        public static AnalyticsResult CustomEvent(string customEventName)
        {
            if (string.IsNullOrEmpty(customEventName))
            {
                throw new ArgumentException("Cannot set custom event name to an empty or null string");
            }
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            return unityAnalyticsHandler.CustomEvent(customEventName);
        }

        public static AnalyticsResult CustomEvent(string customEventName, IDictionary<string, object> eventData)
        {
            if (string.IsNullOrEmpty(customEventName))
            {
                throw new ArgumentException("Cannot set custom event name to an empty or null string");
            }
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            CustomEventData data = new CustomEventData(customEventName);
            data.Add(eventData);
            return unityAnalyticsHandler.CustomEvent(data);
        }

        public static AnalyticsResult CustomEvent(string customEventName, Vector3 position)
        {
            if (string.IsNullOrEmpty(customEventName))
            {
                throw new ArgumentException("Cannot set custom event name to an empty or null string");
            }
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            CustomEventData eventData = new CustomEventData(customEventName);
            eventData.Add("x", (double) Convert.ToDecimal(position.x));
            eventData.Add("y", (double) Convert.ToDecimal(position.y));
            eventData.Add("z", (double) Convert.ToDecimal(position.z));
            return unityAnalyticsHandler.CustomEvent(eventData);
        }

        internal static UnityAnalyticsHandler GetUnityAnalyticsHandler()
        {
            if (s_UnityAnalyticsHandler == null)
            {
                s_UnityAnalyticsHandler = new UnityAnalyticsHandler();
            }
            return s_UnityAnalyticsHandler;
        }

        public static AnalyticsResult SetUserBirthYear(int birthYear)
        {
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (s_UnityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            return unityAnalyticsHandler.SetUserBirthYear(birthYear);
        }

        public static AnalyticsResult SetUserGender(Gender gender)
        {
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            return unityAnalyticsHandler.SetUserGender(gender);
        }

        public static AnalyticsResult SetUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Cannot set userId to an empty or null string");
            }
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            return unityAnalyticsHandler.SetUserId(userId);
        }

        public static AnalyticsResult Transaction(string productId, decimal amount, string currency)
        {
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            return unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, null, null);
        }

        public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature)
        {
            UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
            if (unityAnalyticsHandler == null)
            {
                return AnalyticsResult.NotInitialized;
            }
            return unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature);
        }
    }
}

