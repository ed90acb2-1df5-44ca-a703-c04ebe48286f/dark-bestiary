using DarkBestiary.Managers;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class BookView : Singleton<BookView>
    {
        [SerializeField] private TextMeshProUGUI textLeft;
        [SerializeField] private TextMeshProUGUI textRight;
        [SerializeField] private Interactable nextButton;
        [SerializeField] private Interactable prevButton;
        [SerializeField] private Interactable closeButton;

        [Header("Sounds")]
        [FMODUnity.EventRef]
        [SerializeField] private string openSound;

        [FMODUnity.EventRef]
        [SerializeField] private string closeSound;

        [FMODUnity.EventRef]
        [SerializeField] private string turnPageSound;

        private void Start()
        {
            this.closeButton.PointerUp += Hide;
            this.nextButton.PointerUp += NextPage;
            this.prevButton.PointerUp += PrevPage;

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
            this.textLeft.text = text;
            this.textLeft.pageToDisplay = 1;
            this.textRight.text = text;
            this.textRight.pageToDisplay = 2;
        }

        private void NextPage()
        {
            SetPage(this.textLeft.pageToDisplay + 2);
        }

        private void PrevPage()
        {
            SetPage(this.textLeft.pageToDisplay - 2);
        }

        private void SetPage(int page)
        {
            AudioManager.Instance.PlayOneShot(this.turnPageSound);

            this.textLeft.pageToDisplay = Mathf.Clamp(page, 1, this.textLeft.textInfo.pageCount);
            this.textRight.pageToDisplay = this.textLeft.pageToDisplay + 1;
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