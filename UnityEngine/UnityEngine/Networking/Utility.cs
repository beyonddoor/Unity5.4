namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    public class Utility
    {
        private static Dictionary<NetworkID, NetworkAccessToken> s_dictTokens = new Dictionary<NetworkID, NetworkAccessToken>();

        private Utility()
        {
        }

        public static NetworkAccessToken GetAccessTokenForNetwork(NetworkID netId)
        {
            NetworkAccessToken token;
            if (!s_dictTokens.TryGetValue(netId, out token))
            {
                token = new NetworkAccessToken();
            }
            return token;
        }

        [Obsolete("This function is unused and should not be referenced in code. Please sign in and setup your project in the editor instead.", true)]
        public static AppID GetAppID()
        {
            return AppID.Invalid;
        }

        public static SourceID GetSourceID()
        {
            return (SourceID) SystemInfo.deviceUniqueIdentifier.GetHashCode();
        }

        public static void SetAccessTokenForNetwork(NetworkID netId, NetworkAccessToken accessToken)
        {
            if (s_dictTokens.ContainsKey(netId))
            {
                s_dictTokens.Remove(netId);
            }
            s_dictTokens.Add(netId, accessToken);
        }

        [Obsolete("This function is unused and should not be referenced in code. Please sign in and setup your project in the editor instead.", true)]
        public static void SetAppID(AppID newAppID)
        {
        }

        [Obsolete("This property is unused and should not be referenced in code.", true)]
        public static bool useRandomSourceID
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
    }
}

