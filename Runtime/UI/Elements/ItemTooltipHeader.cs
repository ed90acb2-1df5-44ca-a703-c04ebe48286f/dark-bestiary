using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipHeader : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;

        public void Construct(Sprite icon, string name)
        {
            m_Icon.sprite = icon;
            m_NameText.text = name;
        }
    }
}