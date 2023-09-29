using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class UiMessagesFrameMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;

        public void Initialize(string text)
        {
            m_Text.text = text;

            Timer.Instance.Wait(
                3.0f,
                delegate
                {
                    m_Text.CrossFadeAlpha(0, 1.0f, true);
                    Timer.Instance.Wait(1.0f, delegate { Destroy(gameObject); });
                }
            );
        }
    }
}