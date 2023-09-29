using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ObjectWithText : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;

        public void ChangeTitle(string title)
        {
            m_Text.text = title;
        }
    }
}