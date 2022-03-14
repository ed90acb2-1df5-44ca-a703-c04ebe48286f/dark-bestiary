using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SpecializationIcon : MonoBehaviour, IPointerClickHandler
    {
        public event Payload<SpecializationIcon> Clicked;

        public Specialization Specialization { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI levelText;

        public void Construct(Specialization specialization)
        {
            Specialization = specialization;

            this.icon.sprite = Resources.Load<Sprite>(Specialization.Data.Icon);
            this.label.text = I18N.Instance.Translate(Specialization.Data.NameKey);
            this.levelText.text = specialization.Experience.Level.ToString();
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}