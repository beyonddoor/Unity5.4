namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    internal class JoinMatchResponse : BasicResponse
    {
        public override void Parse(object obj)
        {
            base.Parse(obj);
            IDictionary<string, object> dictJsonObj = obj as IDictionary<string, object>;
            if (dictJsonObj == null)
            {
                throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
            }
            this.address = base.ParseJSONString("address", obj, dictJsonObj);
            this.port = base.ParseJSONInt32("port", obj, dictJsonObj);
            this.networkId = (NetworkID) base.ParseJSONUInt64("networkId", obj, dictJsonObj);
            this.accessTokenString = base.ParseJSONString("accessTokenString", obj, dictJsonObj);
            this.nodeId = (NodeID) base.ParseJSONUInt16("nodeId", obj, dictJsonObj);
            this.usingRelay = base.ParseJSONBool("usingRelay", obj, dictJsonObj);
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.address, this.port, this.networkId.ToString("X"), string.IsNullOrEmpty(this.accessTokenString), this.nodeId.ToString("X"), this.usingRelay };
            return UnityString.Format("[{0}]-address:{1},port:{2},networkId:0x{3},accessTokenString.IsEmpty:{4},nodeId:0x{5},usingRelay:{6}", args);
        }

        public string accessTokenString { get; set; }

        public string address { get; set; }

        public int domain { get; set; }

        public NetworkID networkId { get; set; }

        public NodeID nodeId { get; set; }

        public int port { get; set; }

        public bool usingRelay { get; set; }
    }
}

