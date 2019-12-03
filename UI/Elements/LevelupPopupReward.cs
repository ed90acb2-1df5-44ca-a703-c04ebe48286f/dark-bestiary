using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LevelupPopupReward : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI text;

        public void Construct(Sprite icon, string label, string text)
        {
            this.icon.sprite = icon;
            this.label.text = label;
            this.text.text = text;
        }
    }
}