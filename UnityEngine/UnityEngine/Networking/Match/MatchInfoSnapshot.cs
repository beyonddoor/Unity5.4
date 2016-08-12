namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine.Networking.Types;

    public class MatchInfoSnapshot
    {
        public MatchInfoSnapshot()
        {
        }

        internal MatchInfoSnapshot(MatchDesc matchDesc)
        {
            this.networkId = matchDesc.networkId;
            this.hostNodeId = matchDesc.hostNodeId;
            this.name = matchDesc.name;
            this.averageEloScore = matchDesc.averageEloScore;
            this.maxSize = matchDesc.maxSize;
            this.currentSize = matchDesc.currentSize;
            this.isPrivate = matchDesc.isPrivate;
            this.matchAttributes = matchDesc.matchAttributes;
            this.directConnectInfos = new List<MatchInfoDirectConnectSnapshot>();
            foreach (MatchDirectConnectInfo info in matchDesc.directConnectInfos)
            {
                this.directConnectInfos.Add(new MatchInfoDirectConnectSnapshot(info));
            }
        }

        public int averageEloScore { get; private set; }

        public int currentSize { get; private set; }

        public List<MatchInfoDirectConnectSnapshot> directConnectInfos { get; private set; }

        public NodeID hostNodeId { get; private set; }

        public bool isPrivate { get; private set; }

        public Dictionary<string, long> matchAttributes { get; private set; }

        public int maxSize { get; private set; }

        public string name { get; private set; }

        public NetworkID networkId { get; private set; }

        public class MatchInfoDirectConnectSnapshot
        {
            public MatchInfoDirectConnectSnapshot()
            {
            }

            internal MatchInfoDirectConnectSnapshot(MatchDirectConnectInfo matchDirectConnectInfo)
            {
                this.nodeId = matchDirectConnectInfo.nodeId;
                this.publicAddress = matchDirectConnectInfo.publicAddress;
                this.privateAddress = matchDirectConnectInfo.privateAddress;
                this.hostPriority = matchDirectConnectInfo.hostPriority;
            }

            public HostPriority hostPriority { get; private set; }

            public NodeID nodeId { get; private set; }

            public string privateAddress { get; private set; }

            public string publicAddress { get; private set; }
        }
    }
}

