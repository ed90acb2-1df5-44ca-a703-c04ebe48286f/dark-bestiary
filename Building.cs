using UnityEngine;

namespace DarkBestiary
{
    public class Building : InteractableObject
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite normal;
        [SerializeField] private Sprite hover;
        [SerializeField] private GameObject questionMark;

        public void ShowQuestionMark()
        {
            this.questionMark.gameObject.SetActive(true);
        }

        public void HideQuestionMark()
        {
            this.questionMark.gameObject.SetActive(false);
        }

        protected override void OnPointerEnter()
        {
            this.spriteRenderer.sprite = this.hover;
        }

        protected override void OnPointerExit()
        {
            this.spriteRenderer.sprite = this.normal;
        }
    }
}