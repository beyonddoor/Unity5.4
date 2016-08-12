namespace UnityEngine.Networking.Match
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    internal class JoinMatchRequest : Request
    {
        public override bool IsValid()
        {
            return (base.IsValid() && (this.networkId != NetworkID.Invalid));
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.networkId.ToString("X"), this.publicAddress, this.privateAddress, this.eloScore, !string.IsNullOrEmpty(this.password) ? "YES" : "NO" };
            return UnityString.Format("[{0}]-networkId:0x{1},publicAddress:{2},privateAddress:{3},eloScore:{4},HasPassword:{5}", args);
        }

        public int eloScore { get; set; }

        public NetworkID networkId { get; set; }

        public string password { get; set; }

        public string privateAddress { get; set; }

        public string publicAddress { get; set; }
    }
}

