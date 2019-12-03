using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelRelic : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Interactable hover;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private VictoryPanelExperience experience;

        private Relic relic;

        public void Construct(Relic relic)
        {
            this.relic = relic;

            this.icon.sprite = Resources.Load<Sprite>(relic.Icon);
            this.nameText.text = relic.ColoredName;

            this.hover.PointerEnter += OnPointerEnter;
            this.hover.PointerExit += OnPointerExit;

            this.experience.Construct(relic.Experience.Snapshot);
            this.experience.Simulate();
        }

        private void OnPointerEnter()
        {
            RelicTooltip.Instance.Show(this.relic, this.hover.GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            RelicTooltip.Instance.Hide();
        }
    }
}