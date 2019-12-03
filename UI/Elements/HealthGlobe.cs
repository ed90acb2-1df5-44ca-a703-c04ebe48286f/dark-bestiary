using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class HealthGlobe : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image healthFill;
        [SerializeField] private Image shieldFill;
        [SerializeField] private Animator animator;

        private float currentHealth;
        private float currentShield;
        private float targetHealth;
        private float targetShield;
        private float maximum;
        private float lerp;

        public void Refresh(float health, float shield, float maximum)
        {
            if (this.targetHealth < health)
            {
                this.animator.Play("increase");
                this.lerp = 0.15f;
            }
            else
            {
                this.lerp = 1.0f;
            }

            this.targetHealth = health;
            this.targetShield = shield;
            this.maximum = maximum;
        }

        private void Update()
        {
            if (this.currentHealth != this.targetHealth || this.currentShield != this.targetShield)
            {
                this.currentHealth = Mathf.Lerp(this.currentHealth, this.targetHealth, this.lerp);
                this.currentShield = Mathf.Lerp(this.currentShield, this.targetShield, this.lerp);

                var current = this.currentHealth + this.currentShield;

                this.text.text = $"{(int) current}\n<size=50%>{current / this.maximum:P0}</size>";
                this.healthFill.fillAmount = this.currentHealth / this.maximum;
                this.shieldFill.fillAmount = current / this.maximum;
            }
        }
    }
}