using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class UiMessagesFrameMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void Initialize(string text)
        {
            this.text.text = text;

            Timer.Instance.Wait(
                3.0f,
                delegate
                {
                    this.text.CrossFadeAlpha(0, 1.0f, true);
                    Timer.Instance.Wait(1.0f, delegate { Destroy(gameObject); });
                }
            );
        }
    }
}