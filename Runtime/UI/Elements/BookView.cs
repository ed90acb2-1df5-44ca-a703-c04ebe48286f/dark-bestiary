using DarkBestiary.Managers;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class BookView : Singleton<BookView>
    {
        [SerializeField] private TextMeshProUGUI m_TextLeft;
        [SerializeField] private TextMeshProUGUI m_TextRight;
        [SerializeField] private Interactable m_NextButton;
        [SerializeField] private Interactable m_PrevButton;
        [SerializeField] private Interactable m_CloseButton;

        
        [Header("Sounds")]
        [FMODUnity.EventRef]
        [SerializeField] private string m_OpenSound;

        
        [FMODUnity.EventRef]
        [SerializeField] private string m_CloseSound;

        
        [FMODUnity.EventRef]
        [SerializeField] private string m_TurnPageSound;

        private void Start()
        {
            m_CloseButton.PointerClick += Hide;
            m_NextButton.PointerClick += NextPage;
            m_PrevButton.PointerClick += PrevPage;

            Instance.gameObject.SetActive(false);
        }

        public void Hide()
        {
            AudioManager.Instance.PlayOneShot(m_CloseSound);
            gameObject.SetActive(false);
        }

        public void Show(string text)
        {
            AudioManager.Instance.PlayOneShot(m_OpenSound);

            gameObject.SetActive(true);
            m_TextLeft.text = text;
            m_TextLeft.pageToDisplay = 1;
            m_TextRight.text = text;
            m_TextRight.pageToDisplay = 2;
        }

        private void NextPage()
        {
            SetPage(m_TextLeft.pageToDisplay + 2);
        }

        private void PrevPage()
        {
            SetPage(m_TextLeft.pageToDisplay - 2);
        }

        private void SetPage(int page)
        {
            AudioManager.Instance.PlayOneShot(m_TurnPageSound);

            m_TextLeft.pageToDisplay = Mathf.Clamp(page, 1, m_TextLeft.textInfo.pageCount);
            m_TextRight.pageToDisplay = m_TextLeft.pageToDisplay + 1;
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