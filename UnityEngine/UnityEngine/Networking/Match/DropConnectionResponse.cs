namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    internal class DropConnectionResponse : Response
    {
        public override void Parse(object obj)
        {
            base.Parse(obj);
            IDictionary<string, object> dictJsonObj = obj as IDictionary<string, object>;
            if (dictJsonObj == null)
            {
                throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
            }
            this.networkId = (NetworkID) base.ParseJSONUInt64("networkId", obj, dictJsonObj);
            this.nodeId = (NodeID) base.ParseJSONUInt16("nodeId", obj, dictJsonObj);
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.networkId.ToString("X") };
            return UnityString.Format("[{0}]-networkId:{1}", args);
        }

        public NetworkID networkId { get; set; }

        public NodeID nodeId { get; set; }
    }
}

