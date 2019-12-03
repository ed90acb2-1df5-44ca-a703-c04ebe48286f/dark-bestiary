using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ObjectWithText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void ChangeTitle(string title)
        {
            this.text.text = title;
        }
    }
}