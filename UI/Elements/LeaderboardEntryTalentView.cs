using DarkBestiary.Talents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntryTalentView : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image iconImage;

        private Talent talent;

        public void Construct(Talent talent)
        {
            this.talent = talent;
            this.iconImage.sprite = Resources.Load<Sprite>(talent.Icon);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            Tooltip.Instance.Show(this.talent.Name, this.talent.Description, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }
    }
}