namespace UnityEngine.Windows.Speech
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class GrammarRecognizer : PhraseRecognizer
    {
        public GrammarRecognizer(string grammarFilePath) : this(grammarFilePath, ConfidenceLevel.Medium)
        {
        }

        public GrammarRecognizer(string grammarFilePath, ConfidenceLevel minimumConfidence)
        {
            this.GrammarFilePath = grammarFilePath;
            base.m_Recognizer = base.CreateFromGrammarFile(grammarFilePath, minimumConfidence);
        }

        public string GrammarFilePath { get; private set; }
    }
}

