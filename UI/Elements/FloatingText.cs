using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class FloatingText : PoolableMonoBehaviour
    {
        private const float VerticalVelocity = 35;
        private const float VerticalOffset = 40;
        private const float Lifetime = 0.75f;

        public void Initialize(string text, Color color, Vector2 position)
        {
            GetComponent<RectTransform>().position = position + Vector2.up * VerticalOffset;
            GetComponent<TextMeshProUGUI>().color = color;
            GetComponent<TextMeshProUGUI>().text = text;

            Timer.Instance.Wait(Lifetime, () =>
            {
                GetComponent<Graphic>().CrossFadeAlpha(0, 0.25f, true);
                Timer.Instance.Wait(0.25f, Despawn);
            });
        }

        private void Update()
        {
            transform.position += VerticalVelocity * Time.deltaTime * Vector3.up;
        }
    }
}