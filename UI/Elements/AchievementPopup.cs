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
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Animator animator;

        private bool isDisappearing;

        public void Initialize(Achievement achievement)
        {
            this.icon.sprite = Resources.Load<Sprite>(achievement.Icon);
            this.text.text = achievement.Name;
        }

        private void Start()
        {
            StartCoroutine(DisappearCoroutine());
        }

        private void Disappear()
        {
            if (this.isDisappearing)
            {
                return;
            }

            this.isDisappearing = true;

            this.animator.Play("death");
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