using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipSocket : PoolableMonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Text;

        public void Construct(Sprite sprite, string text)
        {
            m_Icon.sprite = sprite;
            m_Text.text = text;
        }
    }
}