using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipDifference : PoolableMonoBehaviour
    {
        [SerializeField] private Image m_ArrowUp;
        [SerializeField] private Image m_ArrowDown;
        [SerializeField] private TextMeshProUGUI m_Text;

        public void Construct(bool increase, string text)
        {
            m_ArrowUp.gameObject.SetActive(increase);
            m_ArrowDown.gameObject.SetActive(!increase);
            m_Text.text = text;
        }
    }
}