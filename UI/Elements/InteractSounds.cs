using DarkBestiary.Managers;
using FMODUnity;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    [RequireComponent(typeof(Interactable))]
    public class InteractSounds : MonoBehaviour
    {
        [SerializeField] private EventReference mouseClickEventReference;
        [SerializeField] private EventReference mouseEnterEventReference;

        private void Start()
        {
            var interactable = GetComponent<Interactable>();
            interactable.PointerClick += OnPointerClick;
            interactable.PointerEnter += OnPointerEnter;
        }

        private void OnPointerClick()
        {
            if (this.mouseClickEventReference.IsNull || SettingsManager.Instance.DisableUiSounds)
            {
                return;
            }

            AudioManager.Instance.PlayOneShot(this.mouseClickEventReference.Path);
        }

        private void OnPointerEnter()
        {
            if (this.mouseEnterEventReference.IsNull || SettingsManager.Instance.DisableUiSounds)
            {
                return;
            }

            AudioManager.Instance.PlayOneShot(this.mouseEnterEventReference.Path);
        }
    }
}