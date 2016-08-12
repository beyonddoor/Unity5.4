namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public sealed class KeywordRecognizer : PhraseRecognizer
    {
        public KeywordRecognizer(string[] keywords) : this(keywords, ConfidenceLevel.Medium)
        {
        }

        public KeywordRecognizer(string[] keywords, ConfidenceLevel minimumConfidence)
        {
            if (keywords == null)
            {
                throw new ArgumentNullException("keywords");
            }
            if (keywords.Length == 0)
            {
                throw new ArgumentException("At least one keyword must be specified.", "keywords");
            }
            this.Keywords = keywords;
            base.m_Recognizer = base.CreateFromKeywords(keywords, minimumConfidence);
        }

        public IEnumerable<string> Keywords { get; private set; }
    }
}

