using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class HealthFrame : MonoBehaviour
    {
        [SerializeField] private Image m_HealthFiller;
        [SerializeField] private Image m_ShieldFiller;
        [SerializeField] private TextMeshProUGUI m_HealthText;

        public void Refresh(float currentHealth, float currentShield, float maximum)
        {
            var maxHealth = (int) maximum - currentShield;
            var current = currentHealth + currentShield;

            m_HealthText.text = $"{((int) currentHealth).ToString()}/{((int) maxHealth).ToString()} ({(currentHealth / maxHealth * 100).ToString("F0")}%)";
            m_HealthFiller.fillAmount = currentHealth / maximum;
            m_ShieldFiller.fillAmount = current / maximum;

            if (currentShield > 0)
            {
                m_HealthText.text += $" ({((int) currentShield).ToString()})";
            }
        }

        public void SetPoisoned(bool isPoisoned)
        {
            m_HealthFiller.color = isPoisoned ? Color.green : Color.red;
        }
    }
}