using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class KeyValueView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Key;
        [SerializeField] private TextMeshProUGUI m_Value;

        public void Construct(string key, string value)
        {
            m_Key.text = key;
            m_Value.text = value;
        }
    }
}