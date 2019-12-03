using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CircleIcon : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void Construct(Sprite sprite)
        {
            this.image.sprite = sprite;
        }
    }
}