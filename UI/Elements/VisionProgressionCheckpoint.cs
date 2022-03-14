using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class VisionProgressionCheckpoint : MonoBehaviour, IPointerClickHandler
    {
        public event Payload<VisionProgressionCheckpoint> Clicked;

        public int Level { get; private set; }
        public bool IsAvailable { get; private set; }

        [SerializeField] private Image background;
        [SerializeField] private Image outline;
        [SerializeField] private TextMeshProUGUI text;

        public void Construct(int level)
        {
            Level = level;
            this.text.text = StringUtils.ToRomanNumeral(Level);
            SetSelected(false);
            SetAvailable(true);
        }

        public void SetAvailable(bool isAvailable)
        {
            IsAvailable = isAvailable;
            this.background.color = isAvailable ? Color.white : Color.gray;
        }

        public void SetSelected(bool isActive)
        {
            this.outline.gameObject.SetActive(isActive);
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}