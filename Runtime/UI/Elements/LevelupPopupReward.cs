using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LevelupPopupReward : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Label;
        [SerializeField] private TextMeshProUGUI m_Text;

        public void Construct(Sprite icon, string label, string text)
        {
            m_Icon.sprite = icon;
            m_Label.text = label;
            m_Text.text = text;
        }
    }
}