using System;
using DarkBestiary.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelExperience : MonoBehaviour
    {
        private const float c_SimulationTime = 4;

        [SerializeField] private Animator m_Animator;
        [SerializeField] private TextMeshProUGUI m_CurrentLevelText;
        [SerializeField] private TextMeshProUGUI m_NextLevelText;
        [SerializeField] private TextMeshProUGUI m_ExperienceText;
        [SerializeField] private TextMeshProUGUI m_ExperienceGainText;
        [SerializeField] private Image m_Filler;

        private Experience m_Experience;
        private int m_Level;
        private int m_Gain;
        private float m_Start;
        private float m_Current;
        private float m_Obtained;
        private float m_ToObtain;
        private float m_Time;

        private bool m_Simulate;

        public void Construct(Experience experience, int skillPoints = 0)
        {
            m_Experience = experience;
            m_Level = experience.Level;
            m_Gain = experience.Pending;

            m_Obtained = experience.GetObtained();
            m_ToObtain = experience.GetRequired();
            m_Current = experience.Current;
            m_Start = experience.Current;

            m_ExperienceGainText.text = $"{I18N.Instance.Translate("ui_experience")}: +{m_Gain.ToString()}";

            if (skillPoints > 0)
            {
                m_ExperienceGainText.text = $"{I18N.Instance.Translate("ui_skill_points")}: +{skillPoints.ToString()}\n{m_ExperienceGainText.text}";
            }

            UpdateBar();
        }

        public void Simulate()
        {
            m_Simulate = m_Experience.Level < m_Experience.MaxLevel;
        }

        private void UpdateBar()
        {
            var fraction = m_Obtained / m_ToObtain;

            m_Filler.fillAmount = fraction;
            m_CurrentLevelText.text = Math.Min(m_Level, m_Experience.MaxLevel).ToString();
            m_NextLevelText.text = Math.Min(m_Level + 1, m_Experience.MaxLevel).ToString();
            m_ExperienceText.text = $"{m_Obtained:F0} / {m_ToObtain:F0} ({fraction:0%})";
        }

        private void LevelUp()
        {
            m_Level++;

            m_ToObtain = m_Experience.RequiredAtLevel(m_Level + 1) -
                            m_Experience.RequiredAtLevel(m_Level);

            m_Obtained = m_Current - m_Experience.RequiredAtLevel(m_Level);

            m_Animator.Play("levelup");

            if (m_Level == m_Experience.MaxLevel)
            {
                m_Simulate = false;
                m_Obtained = m_ToObtain;
            }
        }

        private void Update()
        {
            if (!m_Simulate || m_Time >= c_SimulationTime)
            {
                return;
            }

            m_Time = Mathf.Min(m_Time + Time.deltaTime, c_SimulationTime);

            var previous = m_Current;
            m_Current = m_Start + m_Gain * Curves.Instance.Logarithmic.Evaluate(m_Time / c_SimulationTime);
            m_Obtained += m_Current - previous;

            if (m_Obtained >= m_ToObtain)
            {
                LevelUp();
            }

            UpdateBar();
        }
    }
}