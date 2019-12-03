using System;
using DarkBestiary.Dialogues;

namespace DarkBestiary.Data
{
    [Serializable]
    public class PhraseData : Identity<int>
    {
        public string Label;
        public string TextKey;
        public Narrator Narrator;

        public float CalculateReadTime()
        {
            return I18N.Instance.Get(this.TextKey).ToString().Length * 0.1f;
        }
    }
}