using DarkBestiary.Managers;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class PageView : Singleton<PageView>
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Interactable closeButton;

        [Header("Sounds")]
        [FMODUnity.EventRef]
        [SerializeField] private string openSound;

        [FMODUnity.EventRef]
        [SerializeField] private string closeSound;

        private void Start()
        {
            this.closeButton.PointerUp += Hide;

            Instance.gameObject.SetActive(false);
        }

        public void Hide()
        {
            AudioManager.Instance.PlayOneShot(this.closeSound);
            gameObject.SetActive(false);
        }

        public void Show(string text)
        {
            AudioManager.Instance.PlayOneShot(this.openSound);
            gameObject.SetActive(true);
            this.text.text = text;
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