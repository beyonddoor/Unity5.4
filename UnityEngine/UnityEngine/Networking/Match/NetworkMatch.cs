namespace UnityEngine.Networking.Match
{
    using SimpleJson;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Types;

    public class NetworkMatch : MonoBehaviour
    {
        private Uri m_BaseUri = new Uri("https://mm.unet.unity3d.com");

        internal Coroutine CreateMatch(CreateMatchRequest req, DataResponseDelegate<MatchInfo> callback)
        {
            if (callback == null)
            {
                UnityEngine.Debug.Log("callback supplied is null, aborting CreateMatch Request.");
                return null;
            }
            Uri uri = new Uri(this.baseUri, "/json/reply/CreateMatchRequest");
            UnityEngine.Debug.Log("MatchMakingClient Create :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("version", Request.currentVersion);
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("accessTokenString", 0);
            form.AddField("domain", req.domain);
            form.AddField("name", req.name);
            form.AddField("size", req.size.ToString());
            form.AddField("advertise", req.advertise.ToString());
            form.AddField("password", req.password);
            form.AddField("publicAddress", req.publicAddress);
            form.AddField("privateAddress", req.privateAddress);
            form.AddField("eloScore", req.eloScore.ToString());
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<CreateMatchResponse, DataResponseDelegate<MatchInfo>>(client, new InternalResponseDelegate<CreateMatchResponse, DataResponseDelegate<MatchInfo>>(this.OnMatchCreate), callback));
        }

        public Coroutine CreateMatch(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, string publicClientAddress, string privateClientAddress, int eloScoreForMatch, int requestDomain, DataResponseDelegate<MatchInfo> callback)
        {
            CreateMatchRequest req = new CreateMatchRequest {
                name = matchName,
                size = matchSize,
                advertise = matchAdvertise,
                password = matchPassword,
                publicAddress = publicClientAddress,
                privateAddress = privateClientAddress,
                eloScore = eloScoreForMatch,
                domain = requestDomain
            };
            return this.CreateMatch(req, callback);
        }

        internal Coroutine DestroyMatch(DestroyMatchRequest req, BasicResponseDelegate callback)
        {
            if (callback == null)
            {
                UnityEngine.Debug.Log("callback supplied is null, aborting DestroyMatch Request.");
                return null;
            }
            Uri uri = new Uri(this.baseUri, "/json/reply/DestroyMatchRequest");
            UnityEngine.Debug.Log("MatchMakingClient Destroy :" + uri.ToString());
            WWWForm form = new WWWForm();
            form.AddField("version", Request.currentVersion);
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
            form.AddField("domain", req.domain);
            form.AddField("networkId", req.networkId.ToString());
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<BasicResponse, BasicResponseDelegate>(client, new InternalResponseDelegate<BasicResponse, BasicResponseDelegate>(this.OnMatchDestroyed), callback));
        }

        public Coroutine DestroyMatch(NetworkID netId, int requestDomain, BasicResponseDelegate callback)
        {
            DestroyMatchRequest req = new DestroyMatchRequest {
                networkId = netId,
                domain = requestDomain
            };
            return this.DestroyMatch(req, callback);
        }

        internal Coroutine DropConnection(DropConnectionRequest req, BasicResponseDelegate callback)
        {
            if (callback == null)
            {
                UnityEngine.Debug.Log("callback supplied is null, aborting DropConnection Request.");
                return null;
            }
            Uri uri = new Uri(this.baseUri, "/json/reply/DropConnectionRequest");
            UnityEngine.Debug.Log("MatchMakingClient DropConnection :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("version", Request.currentVersion);
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
            form.AddField("domain", req.domain);
            form.AddField("networkId", req.networkId.ToString());
            form.AddField("nodeId", req.nodeId.ToString());
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<DropConnectionResponse, BasicResponseDelegate>(client, new InternalResponseDelegate<DropConnectionResponse, BasicResponseDelegate>(this.OnDropConnection), callback));
        }

        public Coroutine DropConnection(NetworkID netId, NodeID dropNodeId, int requestDomain, BasicResponseDelegate callback)
        {
            DropConnectionRequest req = new DropConnectionRequest {
                networkId = netId,
                nodeId = dropNodeId,
                domain = requestDomain
            };
            return this.DropConnection(req, callback);
        }

        internal Coroutine JoinMatch(JoinMatchRequest req, DataResponseDelegate<MatchInfo> callback)
        {
            if (callback == null)
            {
                UnityEngine.Debug.Log("callback supplied is null, aborting JoinMatch Request.");
                return null;
            }
            Uri uri = new Uri(this.baseUri, "/json/reply/JoinMatchRequest");
            UnityEngine.Debug.Log("MatchMakingClient Join :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("version", Request.currentVersion);
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("accessTokenString", 0);
            form.AddField("domain", req.domain);
            form.AddField("networkId", req.networkId.ToString());
            form.AddField("password", req.password);
            form.AddField("publicAddress", req.publicAddress);
            form.AddField("privateAddress", req.privateAddress);
            form.AddField("eloScore", req.eloScore.ToString());
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<JoinMatchResponse, DataResponseDelegate<MatchInfo>>(client, new InternalResponseDelegate<JoinMatchResponse, DataResponseDelegate<MatchInfo>>(this.OnMatchJoined), callback));
        }

        public Coroutine JoinMatch(NetworkID netId, string matchPassword, string publicClientAddress, string privateClientAddress, int eloScoreForClient, int requestDomain, DataResponseDelegate<MatchInfo> callback)
        {
            JoinMatchRequest req = new JoinMatchRequest {
                networkId = netId,
                password = matchPassword,
                publicAddress = publicClientAddress,
                privateAddress = privateClientAddress,
                eloScore = eloScoreForClient,
                domain = requestDomain
            };
            return this.JoinMatch(req, callback);
        }

        internal Coroutine ListMatches(ListMatchRequest req, DataResponseDelegate<List<MatchInfoSnapshot>> callback)
        {
            if (callback == null)
            {
                UnityEngine.Debug.Log("callback supplied is null, aborting ListMatch Request.");
                return null;
            }
            Uri uri = new Uri(this.baseUri, "/json/reply/ListMatchRequest");
            UnityEngine.Debug.Log("MatchMakingClient ListMatches :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("version", Request.currentVersion);
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("accessTokenString", 0);
            form.AddField("domain", req.domain);
            form.AddField("pageSize", req.pageSize);
            form.AddField("pageNum", req.pageNum);
            form.AddField("nameFilter", req.nameFilter);
            form.AddField("filterOutPrivateMatches", req.filterOutPrivateMatches.ToString());
            form.AddField("eloScore", req.eloScore.ToString());
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<ListMatchResponse, DataResponseDelegate<List<MatchInfoSnapshot>>>(client, new InternalResponseDelegate<ListMatchResponse, DataResponseDelegate<List<MatchInfoSnapshot>>>(this.OnMatchList), callback));
        }

        public Coroutine ListMatches(int startPageNumber, int resultPageSize, string matchNameFilter, bool filterOutPrivateMatchesFromResults, int eloScoreTarget, int requestDomain, DataResponseDelegate<List<MatchInfoSnapshot>> callback)
        {
            ListMatchRequest req = new ListMatchRequest {
                pageNum = startPageNumber,
                pageSize = resultPageSize,
                nameFilter = matchNameFilter,
                filterOutPrivateMatches = filterOutPrivateMatchesFromResults,
                eloScore = eloScoreTarget,
                domain = requestDomain
            };
            return this.ListMatches(req, callback);
        }

        internal void OnDropConnection(DropConnectionResponse response, BasicResponseDelegate userCallback)
        {
            userCallback(response.success, response.extendedInfo);
        }

        internal virtual void OnMatchCreate(CreateMatchResponse response, DataResponseDelegate<MatchInfo> userCallback)
        {
            if (response.success)
            {
                Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
            }
            userCallback(response.success, response.extendedInfo, new MatchInfo(response));
        }

        internal void OnMatchDestroyed(BasicResponse response, BasicResponseDelegate userCallback)
        {
            userCallback(response.success, response.extendedInfo);
        }

        internal void OnMatchJoined(JoinMatchResponse response, DataResponseDelegate<MatchInfo> userCallback)
        {
            if (response.success)
            {
                Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
            }
            userCallback(response.success, response.extendedInfo, new MatchInfo(response));
        }

        internal void OnMatchList(ListMatchResponse response, DataResponseDelegate<List<MatchInfoSnapshot>> userCallback)
        {
            List<MatchInfoSnapshot> responseData = new List<MatchInfoSnapshot>();
            foreach (MatchDesc desc in response.matches)
            {
                responseData.Add(new MatchInfoSnapshot(desc));
            }
            userCallback(response.success, response.extendedInfo, responseData);
        }

        internal void OnSetMatchAttributes(BasicResponse response, BasicResponseDelegate userCallback)
        {
            userCallback(response.success, response.extendedInfo);
        }

        [DebuggerHidden]
        private IEnumerator ProcessMatchResponse<JSONRESPONSE, USERRESPONSEDELEGATETYPE>(WWW client, InternalResponseDelegate<JSONRESPONSE, USERRESPONSEDELEGATETYPE> internalCallback, USERRESPONSEDELEGATETYPE userCallback) where JSONRESPONSE: Response, new()
        {
            return new <ProcessMatchResponse>c__Iterator0<JSONRESPONSE, USERRESPONSEDELEGATETYPE> { client = client, internalCallback = internalCallback, userCallback = userCallback, <$>client = client, <$>internalCallback = internalCallback, <$>userCallback = userCallback };
        }

        internal Coroutine SetMatchAttributes(SetMatchAttributesRequest req, BasicResponseDelegate callback)
        {
            if (callback == null)
            {
                UnityEngine.Debug.Log("callback supplied is null, aborting SetMatchAttributes Request.");
                return null;
            }
            Uri uri = new Uri(this.baseUri, "/json/reply/SetMatchAttributesRequest");
            UnityEngine.Debug.Log("MatchMakingClient SetMatchAttributes :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("version", Request.currentVersion);
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
            form.AddField("domain", req.domain);
            form.AddField("networkId", req.networkId.ToString());
            form.AddField("isListed", req.isListed.ToString());
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<BasicResponse, BasicResponseDelegate>(client, new InternalResponseDelegate<BasicResponse, BasicResponseDelegate>(this.OnSetMatchAttributes), callback));
        }

        public Coroutine SetMatchAttributes(NetworkID networkId, bool isListed, int requestDomain, BasicResponseDelegate callback)
        {
            SetMatchAttributesRequest req = new SetMatchAttributesRequest {
                networkId = networkId,
                isListed = isListed,
                domain = requestDomain
            };
            return this.SetMatchAttributes(req, callback);
        }

        [Obsolete("This function is not used any longer to interface with the matchmaker. Please set up your project by logging in through the editor connect dialog.", true)]
        public void SetProgramAppID(AppID programAppID)
        {
        }

        public Uri baseUri
        {
            get
            {
                return this.m_BaseUri;
            }
            set
            {
                this.m_BaseUri = value;
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessMatchResponse>c__Iterator0<JSONRESPONSE, USERRESPONSEDELEGATETYPE> : IDisposable, IEnumerator, IEnumerator<object> where JSONRESPONSE: Response, new()
        {
            internal object $current;
            internal int $PC;
            internal WWW <$>client;
            internal NetworkMatch.InternalResponseDelegate<JSONRESPONSE, USERRESPONSEDELEGATETYPE> <$>internalCallback;
            internal USERRESPONSEDELEGATETYPE <$>userCallback;
            internal IDictionary<string, object> <dictJsonObj>__2;
            internal FormatException <exception>__3;
            internal JSONRESPONSE <jsonInterface>__0;
            internal object <o>__1;
            internal WWW client;
            internal NetworkMatch.InternalResponseDelegate<JSONRESPONSE, USERRESPONSEDELEGATETYPE> internalCallback;
            internal USERRESPONSEDELEGATETYPE userCallback;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = this.client;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<jsonInterface>__0 = Activator.CreateInstance<JSONRESPONSE>();
                        if (!string.IsNullOrEmpty(this.client.error))
                        {
                            object[] args = new object[] { this.client.error, this.client.text };
                            this.<jsonInterface>__0.SetFailure(UnityString.Format("Request error:[{0}] Raw response:[{1}]", args));
                            break;
                        }
                        if (SimpleJson.SimpleJson.TryDeserializeObject(this.client.text, out this.<o>__1))
                        {
                            this.<dictJsonObj>__2 = this.<o>__1 as IDictionary<string, object>;
                            if (this.<dictJsonObj>__2 != null)
                            {
                                try
                                {
                                    this.<jsonInterface>__0.Parse(this.<o>__1);
                                }
                                catch (FormatException exception)
                                {
                                    this.<exception>__3 = exception;
                                    object[] objArray1 = new object[] { this.<exception>__3.ToString() };
                                    this.<jsonInterface>__0.SetFailure(UnityString.Format("FormatException:[{0}] ", objArray1));
                                }
                            }
                        }
                        break;

                    default:
                        goto Label_0153;
                }
                this.client.Dispose();
                this.internalCallback(this.<jsonInterface>__0, this.userCallback);
                this.$PC = -1;
            Label_0153:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public delegate void BasicResponseDelegate(bool success, string extendedInfo);

        public delegate void DataResponseDelegate<T>(bool success, string extendedInfo, T responseData);

        private delegate void InternalResponseDelegate<T, U>(T response, U userCallback);
    }
}

