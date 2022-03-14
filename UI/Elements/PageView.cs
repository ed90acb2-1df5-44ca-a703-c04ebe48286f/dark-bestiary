using DarkBestiary.Managers;
using FMODUnity;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class PageView : Singleton<PageView>
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private EventReference openSoundEventReference;
        [SerializeField] private EventReference closeSoundEventReference;

        private void Start()
        {
            this.closeButton.PointerClick += Hide;

            Instance.gameObject.SetActive(false);
        }

        public void Hide()
        {
            AudioManager.Instance.PlayOneShot(this.closeSoundEventReference.Path);
            gameObject.SetActive(false);
        }

        public void Show(string text)
        {
            AudioManager.Instance.PlayOneShot(this.openSoundEventReference.Path);
            gameObject.SetActive(true);
            this.text.text = text.Replace("\t", "    ");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
    }
}