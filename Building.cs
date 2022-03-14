using UnityEngine;

namespace DarkBestiary
{
    public class Building : InteractableObject
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite normal;
        [SerializeField] private Sprite hover;

        protected override void OnPointerEnter()
        {
            if (this.spriteRenderer == null)
            {
                return;
            }

            this.spriteRenderer.sprite = this.hover;
        }

        protected override void OnPointerExit()
        {
            if (this.spriteRenderer == null)
            {
                return;
            }

            this.spriteRenderer.sprite = this.normal;
        }
    }
}