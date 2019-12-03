using System;
using System.Collections.Generic;
using DarkBestiary.Dialogues;

namespace DarkBestiary.Data
{
    [Serializable]
    public class DialogueData : Identity<int>
    {
        public string TitleKey;
        public string TextKey;
        public bool IsParent;
        public Narrator Narrator;
        public List<int> Validators = new List<int>();
        public List<DialogueActionData> Actions = new List<DialogueActionData>();
    }

    [Serializable]
    public class DialogueActionData
    {
        public string TextKey;
        public string Type;
        public int NextDialogueId;
    }
}