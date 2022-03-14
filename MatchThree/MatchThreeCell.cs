using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.MatchThree
{
    public class MatchThreeCell : PoolableMonoBehaviour
    {
        public event Payload<MatchThreeCell> Clicked;

        public int Id { get; set; }
        public int Index { get; set; }

        public TextMeshProUGUI Text => this.text;

        [SerializeField] private SpriteRenderer graphics;
        [SerializeField] private SpriteRenderer outline;
        [SerializeField] private TextMeshProUGUI text;

        public void Setup(int index, Color color)
        {
            Index = index;
            this.text.text = index.ToString();
            this.graphics.color = color;
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
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        private void OnMouseUpAsButton()
        {
            Clicked?.Invoke(this);
        }
    }
}