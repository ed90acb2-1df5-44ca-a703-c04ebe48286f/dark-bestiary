using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class UrlButton : MonoBehaviour
    {
        [SerializeField] private Button m_Button;
        [SerializeField] private string m_URL;

        private void Start()
        {
            m_Button.onClick.AddListener(() =>
            {
                Application.OpenURL(m_URL);
            });
        }
    }
}