using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillSetButton : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Payload<SkillSetButton> Clicked;

        public SkillSet Set { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image outline;

        public void Construct(SkillSet set)
        {
            Set = set;

            this.icon.sprite = Resources.Load<Sprite>(set.Icon);
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            SkillSetTooltip.Instance.Show(
                CharacterManager.Instance.Character.Entity, Set, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            SkillSetTooltip.Instance.Hide();
        }
    }
}