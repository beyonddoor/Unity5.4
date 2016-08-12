namespace UnityEngine.Networking.Match
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    internal abstract class Request
    {
        public static readonly int currentVersion = 3;

        protected Request()
        {
        }

        public virtual bool IsValid()
        {
            return (this.sourceId != SourceID.Invalid);
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.sourceId.ToString("X"), this.projectId, string.IsNullOrEmpty(this.accessTokenString), this.domain };
            return UnityString.Format("[{0}]-SourceID:0x{1},projectId:{2},accessTokenString.IsEmpty:{3},domain:{4}", args);
        }

        public string accessTokenString { get; set; }

        public AppID appId { get; set; }

        public int domain { get; set; }

        public string projectId { get; set; }

        public SourceID sourceId { get; set; }

        public int version { get; set; }
    }
}

