using System;
using DarkBestiary.Extensions;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.MatchThree
{
    public class MatchThreeCell : PoolableMonoBehaviour
    {
        public event Action<MatchThreeCell> Clicked;

        public int Id { get; set; }
        public int Index { get; set; }

        public TextMeshProUGUI Text => m_Text;

        [SerializeField] private SpriteRenderer m_Graphics;
        [SerializeField] private SpriteRenderer m_Outline;
        [SerializeField] private TextMeshProUGUI m_Text;

        public void Setup(int index, Color color)
        {
            Index = index;
            m_Text.text = index.ToString();
            m_Graphics.color = color;
        }

        private void OnMouseEnter()
        {
            // this.outline.color = this.outline.color.With(a: 1);
        }

        private void OnMouseExit()
        {
            // this.outline.color = this.outline.color.With(a: 0);
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        private void OnMouseUpAsButton()
        {
            Clicked?.Invoke(this);
        }
    }
}