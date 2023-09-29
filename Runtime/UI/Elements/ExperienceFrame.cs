using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ExperienceFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_Filler;
        [SerializeField] private GameObject m_ExperienceTooltip;
        [SerializeField] private TextMeshProUGUI m_ExperienceText;

        public void Refresh(int current, int required)
        {
            var fraction = (float) current / required;

            m_ExperienceText.text = $"{current} / {required} ({fraction:0%})";
            m_Filler.fillAmount = fraction;

            m_ExperienceTooltip.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            m_ExperienceTooltip.SetActive(true);
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            m_ExperienceTooltip.SetActive(false);
        }
    }
}