using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    [RequireComponent(typeof(Interactable))]
    public class InteractSounds : MonoBehaviour
    {
        
        [FMODUnity.EventRef]
        [SerializeField] private string m_MouseClickEvent = "event:/SFX/UI/interactable_Mouse_Click";

        
        [FMODUnity.EventRef]
        [SerializeField] private string m_MouseEnterEvent = "event:/SFX/UI/interactable_Mouse_Enter";

        private void Start()
        {
            var interactable = GetComponent<Interactable>();
            interactable.PointerClick += OnPointerClick;
            interactable.PointerEnter += OnPointerEnter;
        }

        private void OnPointerClick()
        {
            if (string.IsNullOrEmpty(m_MouseClickEvent) || SettingsManager.Instance.DisableUiSounds)
            {
                return;
            }

            AudioManager.Instance.PlayOneShot(m_MouseClickEvent);
        }

        private void OnPointerEnter()
        {
            if (string.IsNullOrEmpty(m_MouseEnterEvent) || SettingsManager.Instance.DisableUiSounds)
            {
                return;
            }

            AudioManager.Instance.PlayOneShot(m_MouseEnterEvent);
        }
    }
}