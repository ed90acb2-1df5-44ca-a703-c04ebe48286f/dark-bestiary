using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ExperienceFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image filler;
        [SerializeField] private GameObject experienceTooltip;
        [SerializeField] private TextMeshProUGUI experienceText;

        public void Refresh(int current, int required)
        {
            var fraction = (float) current / required;

            this.experienceText.text = $"{current} / {required} ({fraction:0%})";
            this.filler.fillAmount = fraction;

            this.experienceTooltip.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            this.experienceTooltip.SetActive(true);
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            this.experienceTooltip.SetActive(false);
        }
    }
}