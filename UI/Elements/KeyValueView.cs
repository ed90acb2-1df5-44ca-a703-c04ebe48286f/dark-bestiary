using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class KeyValueView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI key;
        [SerializeField] private TextMeshProUGUI value;

        public void Construct(string key, string value)
        {
            this.key.text = key;
            this.value.text = value;
        }
    }
}