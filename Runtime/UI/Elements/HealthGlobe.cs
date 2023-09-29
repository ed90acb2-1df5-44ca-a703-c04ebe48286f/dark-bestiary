using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class HealthGlobe : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Image m_HealthFill;
        [SerializeField] private Image m_ShieldFill;
        [SerializeField] private Animator m_Animator;

        private float m_CurrentHealth;
        private float m_CurrentShield;
        private float m_TargetHealth;
        private float m_TargetShield;
        private float m_Maximum;
        private float m_Lerp;

        public void Refresh(float health, float shield, float maximum)
        {
            if (m_TargetHealth < health)
            {
                m_Animator.Play("increase");
                m_Lerp = 0.15f;
            }
            else
            {
                m_Lerp = 1.0f;
            }

            m_TargetHealth = health;
            m_TargetShield = shield;
            m_Maximum = maximum;
        }

        private void Update()
        {
            if (m_CurrentHealth != m_TargetHealth || m_CurrentShield != m_TargetShield)
            {
                m_CurrentHealth = Mathf.Lerp(m_CurrentHealth, m_TargetHealth, m_Lerp);
                m_CurrentShield = Mathf.Lerp(m_CurrentShield, m_TargetShield, m_Lerp);

                var current = m_CurrentHealth + m_CurrentShield;

                m_Text.text = $"{(int) current}\n<size=50%>{current / m_Maximum:P0}</size>";
                m_HealthFill.fillAmount = m_CurrentHealth / m_Maximum;
                m_ShieldFill.fillAmount = current / m_Maximum;
            }
        }
    }
}