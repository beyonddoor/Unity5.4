namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    internal class MatchDirectConnectInfo : ResponseBase
    {
        public override void Parse(object obj)
        {
            IDictionary<string, object> dictJsonObj = obj as IDictionary<string, object>;
            if (dictJsonObj == null)
            {
                throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
            }
            this.nodeId = (NodeID) base.ParseJSONUInt16("nodeId", obj, dictJsonObj);
            this.publicAddress = base.ParseJSONString("publicAddress", obj, dictJsonObj);
            this.privateAddress = base.ParseJSONString("privateAddress", obj, dictJsonObj);
            this.hostPriority = (HostPriority) base.ParseJSONInt32("hostPriority", obj, dictJsonObj);
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.nodeId, this.publicAddress, this.privateAddress, this.hostPriority };
            return UnityString.Format("[{0}]-nodeId:{1},publicAddress:{2},privateAddress:{3},hostPriority:{4}", args);
        }

        public HostPriority hostPriority { get; set; }

        public NodeID nodeId { get; set; }

        public string privateAddress { get; set; }

        public string publicAddress { get; set; }
    }
}

