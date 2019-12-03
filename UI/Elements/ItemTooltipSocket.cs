using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipSocket : PoolableMonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;

        public void Construct(Sprite sprite, string text)
        {
            this.icon.sprite = sprite;
            this.text.text = text;
        }
    }
}