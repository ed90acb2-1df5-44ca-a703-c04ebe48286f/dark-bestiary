using System.Collections;
using DarkBestiary.Achievements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class AchievementPopup : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Animator m_Animator;

        private bool m_IsDisappearing;

        public void Initialize(Achievement achievement)
        {
            m_Icon.sprite = Resources.Load<Sprite>(achievement.Icon);
            m_Text.text = achievement.Name;
        }

        private void Start()
        {
            StartCoroutine(DisappearCoroutine());
        }

        private void Disappear()
        {
            if (m_IsDisappearing)
            {
                return;
            }

            m_IsDisappearing = true;

            m_Animator.Play("death");
            Destroy(gameObject, 1.0f);
        }

        private IEnumerator DisappearCoroutine()
        {
            yield return new WaitForSeconds(3.0f);

            Disappear();
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Disappear();
        }
    }
}