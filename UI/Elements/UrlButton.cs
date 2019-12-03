using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class UrlButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private string url;

        private void Start()
        {
            this.button.onClick.AddListener(() =>
            {
                Application.OpenURL(this.url);
            });
        }
    }
}