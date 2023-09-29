using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class FloatingText : PoolableMonoBehaviour
    {
        private const float c_VerticalVelocity = 35;
        private const float c_VerticalOffset = 40;
        private const float c_Lifetime = 0.75f;

        public void Initialize(string text, Color color, Vector2 position)
        {
            GetComponent<RectTransform>().position = position + Vector2.up * c_VerticalOffset;
            var textMesh = GetComponent<TextMeshProUGUI>();
            textMesh.color = color;
            textMesh.text = text;

            Timer.Instance.Wait(c_Lifetime, Despawn);
        }

        private void Update()
        {
            transform.position += c_VerticalVelocity * Time.deltaTime * Vector3.up;
        }
    }
}