using DarkBestiary.Talents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntryTalentView : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_IconImage;

        private Talent m_Talent;

        public void Construct(Talent talent)
        {
            m_Talent = talent;
            m_IconImage.sprite = Resources.Load<Sprite>(talent.Icon);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            Tooltip.Instance.Show(m_Talent.Name, m_Talent.Description, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }
    }
}