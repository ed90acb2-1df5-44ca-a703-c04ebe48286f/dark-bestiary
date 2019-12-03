using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    [RequireComponent(typeof(Interactable))]
    public class InteractSounds : MonoBehaviour
    {
        [FMODUnity.EventRef]
        [SerializeField] private string mouseClickEvent = "event:/SFX/UI/interactable_Mouse_Click";

        [FMODUnity.EventRef]
        [SerializeField] private string mouseEnterEvent = "event:/SFX/UI/interactable_Mouse_Enter";

        private void Start()
        {
            var interactable = GetComponent<Interactable>();
            interactable.PointerDown += OnPointerDown;
            interactable.PointerEnter += OnPointerEnter;
        }

        private void OnPointerDown()
        {
            if (string.IsNullOrEmpty(this.mouseClickEvent))
            {
                return;
            }

            AudioManager.Instance.PlayOneShot(this.mouseClickEvent);
        }

        private void OnPointerEnter()
        {
            if (string.IsNullOrEmpty(this.mouseEnterEvent))
            {
                return;
            }

            AudioManager.Instance.PlayOneShot(this.mouseEnterEvent);
        }
    }
}