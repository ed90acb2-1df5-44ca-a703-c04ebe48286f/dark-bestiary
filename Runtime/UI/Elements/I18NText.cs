using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class I18NText : MonoBehaviour
    {
        [SerializeField] private string m_Key;

        private void Start()
        {
            if (string.IsNullOrEmpty(m_Key))
            {
                return;
            }

            GetComponent<TextMeshProUGUI>().text = I18N.Instance.Get(m_Key);
        }
    }
}