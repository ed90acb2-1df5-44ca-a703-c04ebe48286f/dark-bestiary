using System.Collections;
using DarkBestiary.Components;
using DarkBestiary.Scenarios.Encounters;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TurnNotificationFrame : Singleton<TurnNotificationFrame>
    {
        [SerializeField] private TextMeshProUGUI text;

        private CanvasGroup canvasGroup;
        private int previousTeamId;

        private void Start()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();
            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.interactable = false;
            this.canvasGroup.alpha = 0;

            CombatEncounter.AnyCombatTurnStarted += OnAnyCombatTurnStarted;
        }

        private void OnAnyCombatTurnStarted(GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();

            if (unit.TeamId == this.previousTeamId)
            {
                return;
            }

            this.previousTeamId = unit.TeamId;
            this.text.text = unit.TeamId == 1 ? I18N.Instance.Get("ui_your_turn") : I18N.Instance.Get("ui_enemy_turn");
            this.text.text = this.text.text.ToUpper();

            StopAllCoroutines();
            StartCoroutine(FadeInAndOutCoroutine());
        }

        private IEnumerator FadeInAndOutCoroutine()
        {
            yield return FadeInCoroutine();
            yield return new WaitForSeconds(1);
            yield return FadeOutCoroutine();
        }

        private IEnumerator FadeInCoroutine()
        {
            while (this.canvasGroup.alpha < 1)
            {
                this.canvasGroup.alpha += 2 * Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator FadeOutCoroutine()
        {
            while (this.canvasGroup.alpha > 0)
            {
                this.canvasGroup.alpha -= 2 * Time.deltaTime;
                yield return null;
            }
        }
    }
}