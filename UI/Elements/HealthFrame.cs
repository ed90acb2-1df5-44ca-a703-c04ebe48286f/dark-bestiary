using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class HealthFrame : MonoBehaviour
    {
        [SerializeField] private Image healthFiller;
        [SerializeField] private Image shieldFiller;
        [SerializeField] private TextMeshProUGUI healthText;

        public void Refresh(float currentHealth, float currentShield, float maximum)
        {
            var maxHealth = (int) maximum - currentShield;
            var current = currentHealth + currentShield;

            this.healthText.text = $"{(int) currentHealth}/{(int) maxHealth} ({currentHealth / maxHealth:P0})";
            this.healthFiller.fillAmount = currentHealth / maximum;
            this.shieldFiller.fillAmount = current / maximum;

            if (currentShield > 0)
            {
                this.healthText.text += $" ({(int) currentShield})";
            }
        }

        public void SetPoisoned(bool isPoisoned)
        {
            this.healthFiller.color = isPoisoned ? Color.green : Color.red;
        }
    }
}