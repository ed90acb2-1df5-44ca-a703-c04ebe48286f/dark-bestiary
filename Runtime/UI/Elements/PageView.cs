using DarkBestiary.Managers;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class PageView : Singleton<PageView>
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Interactable m_CloseButton;

        
        [Header("Sounds")]
        [FMODUnity.EventRef]
        [SerializeField] private string m_OpenSound;

        
        [FMODUnity.EventRef]
        [SerializeField] private string m_CloseSound;

        private void Start()
        {
            m_CloseButton.PointerClick += Hide;

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
            m_Text.text = text.Replace("\t", "    ");
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