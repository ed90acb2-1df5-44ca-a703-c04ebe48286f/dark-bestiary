using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntryRelicView : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private Interactable m_Hover;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_ExperienceLevelText;
        [SerializeField] private Image m_ExperienceFiller;

        private Relic m_Relic;

        public void Change(Relic relic)
        {
            m_Relic = relic;

            m_Icon.sprite = Resources.Load<Sprite>(relic.Icon);
            m_Icon.gameObject.SetActive(!m_Relic.IsEmpty);

            m_NameText.gameObject.SetActive(!m_Relic.IsEmpty);
            m_ExperienceLevelText.gameObject.SetActive(!m_Relic.IsEmpty);
            m_ExperienceFiller.fillAmount = 0;

            if (m_Relic.IsEmpty)
            {
                return;
            }

            m_NameText.text = relic.ColoredName;
            m_ExperienceLevelText.text = relic.Experience.Level.ToString();
            m_ExperienceFiller.fillAmount = relic.Experience.GetObtainedFraction();

            m_Hover.PointerEnter += OnPointerEnter;
            m_Hover.PointerExit += OnPointerExit;
        }

        private void OnPointerEnter()
        {
            if (m_Relic.IsEmpty)
            {
                return;
            }

            RelicTooltip.Instance.Show(m_Relic, m_Hover.GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            RelicTooltip.Instance.Hide();
        }
    }
}