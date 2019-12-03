using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipHeader : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;

        public void Construct(Sprite icon, string name)
        {
            this.icon.sprite = icon;
            this.nameText.text = name;
        }
    }
}