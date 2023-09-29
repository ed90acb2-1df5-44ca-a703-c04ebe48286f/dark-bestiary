using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class DamageMeterRow : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DamageText;
        [SerializeField] private Image m_Bar;

        private Color? m_DefaultColor;

        public void UpdateProperties(string name, string damage, float progress, bool isAlive)
        {
            m_DefaultColor ??= m_Bar.color;

            m_NameText.text = name;
            m_DamageText.text = damage;
            m_Bar.fillAmount = progress;
            m_Bar.color = isAlive ? m_DefaultColor.Value : new Color(0.3f, 0.3f, 0.3f);
        }
    }
}