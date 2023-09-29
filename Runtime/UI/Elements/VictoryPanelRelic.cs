using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelRelic : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private Interactable m_Hover;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private VictoryPanelExperience m_Experience;

        private Relic m_Relic;

        public void Construct(Relic relic)
        {
            m_Relic = relic;

            m_Icon.sprite = Resources.Load<Sprite>(relic.Icon);
            m_NameText.text = relic.ColoredName;

            m_Hover.PointerEnter += OnPointerEnter;
            m_Hover.PointerExit += OnPointerExit;

            m_Experience.Construct(relic.Experience.Snapshot);
            m_Experience.Simulate();
        }

        private void OnPointerEnter()
        {
            RelicTooltip.Instance.Show(m_Relic, m_Hover.GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            RelicTooltip.Instance.Hide();
        }
    }
}