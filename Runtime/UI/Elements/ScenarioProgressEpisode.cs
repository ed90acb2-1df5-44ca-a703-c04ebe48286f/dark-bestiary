using DarkBestiary.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioProgressEpisode : MonoBehaviour
    {
        [SerializeField] private Image m_Background;
        [SerializeField] private Image m_Checkmark;

        
        [Space(10)]
        [Header("Colors")]
        [SerializeField] private Color m_DefaultColor;
        [SerializeField] private Color m_InactiveColor;

        public void SetCurrent()
        {
            m_Background.color = m_DefaultColor;
            m_Checkmark.color = m_Checkmark.color.With(a: 0);
        }

        public void SetCompleted()
        {
            m_Background.color = m_DefaultColor;
            m_Checkmark.color = m_Checkmark.color.With(a: 1);
        }

        public void SetInactive()
        {
            m_Background.color = m_InactiveColor;
            m_Checkmark.color = m_Checkmark.color.With(a: 0);
        }
    }
}