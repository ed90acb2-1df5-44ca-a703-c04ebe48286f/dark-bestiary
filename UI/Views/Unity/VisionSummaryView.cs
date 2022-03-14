using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class VisionSummaryView : View, IVisionSummaryView
    {
        public event Payload CompleteButtonClicked;

        [Header("Colors")]
        [SerializeField] private Color headerTextColorSuccess;
        [SerializeField] private Color headerTextColorFailure;

        [Header("Stuff")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI characterText;
        [SerializeField] private Interactable completeButton;
        [SerializeField] private GameObject particles;

        [Header("Prefabs")]
        [SerializeField] private VictoryPanelReward rewardPrefab;
        [SerializeField] private Transform rewardContainer;
        [SerializeField] private KeyValueView summaryRowPrefab;
        [SerializeField] private Transform summaryRowContainer;

        public void Construct(List<Item> rewards, IEnumerable<KeyValuePair<string, string>> summary)
        {
            var character = CharacterManager.Instance.Character;
            var characterLevel = character.Entity.GetComponent<ExperienceComponent>().Experience.Level;

            this.characterText.text = $"{character.Name}\n<size=75%>{I18N.Instance.Translate("ui_level")} {characterLevel}</size>";
            this.completeButton.PointerClick += OnCompleteButtonClicked;

            foreach (var pair in summary)
            {
                Instantiate(this.summaryRowPrefab, this.summaryRowContainer).Construct(pair.Key, pair.Value);
            }

            foreach (var reward in rewards)
            {
                Instantiate(this.rewardPrefab, this.rewardContainer).Construct(reward, false);
            }

            this.rewardContainer.gameObject.SetActive(rewards.Count > 0);
        }

        public void SetSuccess(bool isSuccess)
        {
            CharacterManager.Instance.Character.Entity.GetComponent<ActorComponent>().PlayAnimation(isSuccess ? "idle" : "death");

            this.particles.SetActive(isSuccess);

            if (isSuccess)
            {
                this.headerText.color = this.headerTextColorSuccess;
                this.headerText.text = I18N.Instance.Translate("ui_challenge_completed");
            }
            else
            {
                this.headerText.color = this.headerTextColorFailure;
                this.headerText.text = I18N.Instance.Translate("ui_challenge_failed");
            }
        }

        private void OnCompleteButtonClicked()
        {
            CompleteButtonClicked?.Invoke();
        }
    }
}