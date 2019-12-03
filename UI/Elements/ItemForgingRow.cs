using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ItemForgingRow : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI value;
        [SerializeField] private TextMeshProUGUI label;

        public void Construct(string value, string label)
        {
            this.value.text = value;
            this.label.text = label;
        }
    }
}