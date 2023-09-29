using UnityEngine;

namespace DarkBestiary
{
    public class KeyBindingInfo
    {
        public KeyType Type;
        public KeyCode Code;
        public string Label;

        public KeyBindingInfo(KeyType type, KeyCode code, string label)
        {
            Type = type;
            Code = code;
            Label = label;
        }
    }
}