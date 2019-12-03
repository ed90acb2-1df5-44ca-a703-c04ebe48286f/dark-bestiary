using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class I18NText : MonoBehaviour
    {
        [SerializeField] private string key;

        private void Start()
        {
            if (string.IsNullOrEmpty(this.key))
            {
                return;
            }

            GetComponent<TextMeshProUGUI>().text = I18N.Instance.Get(this.key);
        }
    }
}