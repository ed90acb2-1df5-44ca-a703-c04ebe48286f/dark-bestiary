using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntryRelicView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Interactable hover;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI experienceLevelText;
        [SerializeField] private Image experienceFiller;

        private Relic relic;

        public void Change(Relic relic)
        {
            this.relic = relic;

            this.icon.sprite = Resources.Load<Sprite>(relic.Icon);
            this.icon.gameObject.SetActive(!this.relic.IsEmpty);

            this.nameText.gameObject.SetActive(!this.relic.IsEmpty);
            this.experienceLevelText.gameObject.SetActive(!this.relic.IsEmpty);
            this.experienceFiller.fillAmount = 0;

            if (this.relic.IsEmpty)
            {
                return;
            }

            this.nameText.text = relic.ColoredName;
            this.experienceLevelText.text = relic.Experience.Level.ToString();
            this.experienceFiller.fillAmount = relic.Experience.GetObtainedFraction();

            this.hover.PointerEnter += OnPointerEnter;
            this.hover.PointerExit += OnPointerExit;
        }

        private void OnPointerEnter()
        {
            if (this.relic.IsEmpty)
            {
                return;
            }

            RelicTooltip.Instance.Show(this.relic, this.hover.GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            RelicTooltip.Instance.Hide();
        }
    }
}