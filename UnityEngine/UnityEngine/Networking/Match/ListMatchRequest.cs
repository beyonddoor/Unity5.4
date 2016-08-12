namespace UnityEngine.Networking.Match
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class ListMatchRequest : Request
    {
        [Obsolete("This bool is deprecated in favor of filterOutPrivateMatches")]
        public bool includePasswordMatches;

        public override bool IsValid()
        {
            int num = (this.matchAttributeFilterLessThan != null) ? this.matchAttributeFilterLessThan.Count : 0;
            num += (this.matchAttributeFilterEqualTo != null) ? this.matchAttributeFilterEqualTo.Count : 0;
            num += (this.matchAttributeFilterGreaterThan != null) ? this.matchAttributeFilterGreaterThan.Count : 0;
            return ((base.IsValid() && ((this.pageSize >= 1) || (this.pageSize <= 0x3e8))) && (num <= 10));
        }

        public override string ToString()
        {
            object[] args = new object[] { base.ToString(), this.pageSize, this.pageNum, this.nameFilter, this.filterOutPrivateMatches, this.eloScore, (this.matchAttributeFilterLessThan != null) ? this.matchAttributeFilterLessThan.Count : 0, (this.matchAttributeFilterEqualTo != null) ? this.matchAttributeFilterEqualTo.Count : 0, (this.matchAttributeFilterGreaterThan != null) ? this.matchAttributeFilterGreaterThan.Count : 0 };
            return UnityString.Format("[{0}]-pageSize:{1},pageNum:{2},nameFilter:{3}, filterOutPrivateMatches:{4}, eloScore:{5}, matchAttributeFilterLessThan.Count:{6}, matchAttributeFilterEqualTo.Count:{7}, matchAttributeFilterGreaterThan.Count:{8}", args);
        }

        public int eloScore { get; set; }

        public bool filterOutPrivateMatches { get; set; }

        public Dictionary<string, long> matchAttributeFilterEqualTo { get; set; }

        public Dictionary<string, long> matchAttributeFilterGreaterThan { get; set; }

        public Dictionary<string, long> matchAttributeFilterLessThan { get; set; }

        public string nameFilter { get; set; }

        public int pageNum { get; set; }

        public int pageSize { get; set; }
    }
}

