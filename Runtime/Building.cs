using UnityEngine;

namespace DarkBestiary
{
    public class Building : InteractableObject
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Sprite m_Normal;
        [SerializeField] private Sprite m_Hover;

        protected override void OnPointerEnter()
        {
            if (m_SpriteRenderer == null)
            {
                return;
            }

            m_SpriteRenderer.sprite = m_Hover;
        }

        protected override void OnPointerExit()
        {
            if (m_SpriteRenderer == null)
            {
                return;
            }

            m_SpriteRenderer.sprite = m_Normal;
        }
    }
}