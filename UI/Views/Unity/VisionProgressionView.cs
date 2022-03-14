using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class VisionProgressionView : View, IVisionProgressionView
    {
        public event Payload CompleteButtonClicked;

        [SerializeField] private Interactable completeButton;
        [SerializeField] private VisionProgressionReward rewardPrefab;
        [SerializeField] private Transform rewardContainer;
        [SerializeField] private VisionProgressionCheckpoint checkpointPrefab;
        [SerializeField] private Transform checkpointContainer;
        [SerializeField] private VictoryPanelExperience victoryPanelExperience;
        [SerializeField] private Image progressBar;

        private readonly List<VisionProgressionCheckpoint> checkpoints = new List<VisionProgressionCheckpoint>();
        private readonly List<VisionProgressionReward> rewards = new List<VisionProgressionReward>();

        private VisionProgressionCheckpoint selectedCheckpoint;
        private VisionProgression progression;

        public void Construct(VisionProgression progression)
        {
            this.progression = progression;
            this.completeButton.PointerClick += OnCompleteButtonPointerClick;

            for (var i = 1; i <= progression.Experience.MaxLevel; i++)
            {
                var checkpoint = Instantiate(this.checkpointPrefab, this.checkpointContainer);
                checkpoint.Construct(i);
                checkpoint.SetAvailable(i <= progression.Experience.Level);
                checkpoint.Clicked += OnCheckpointClicked;
                this.checkpoints.Add(checkpoint);
            }

            this.victoryPanelExperience.Construct(progression.Experience.Snapshot);
            Timer.Instance.Wait(1, () => {this.victoryPanelExperience.Simulate();});

            this.progressBar.fillAmount = (progression.Experience.Level - 1) * (1.0f / (progression.Experience.MaxLevel - 1));

            OnCheckpointClicked(this.checkpoints[this.progression.Experience.Level - 1]);
        }

        private void OnCheckpointClicked(VisionProgressionCheckpoint checkpoint)
        {
            if (!checkpoint.IsAvailable || checkpoint == this.selectedCheckpoint)
            {
                return;
            }

            this.selectedCheckpoint?.SetSelected(false);
            this.selectedCheckpoint = checkpoint;
            this.selectedCheckpoint.SetSelected(true);

            DisplayTierRewards(checkpoint.Level);
        }

        private void DisplayTierRewards(int tier)
        {
            foreach (var reward in this.rewards)
            {
                Destroy(reward.gameObject);
            }

            this.rewards.Clear();

            foreach (var talent in this.progression.Talents.Tiers[tier - 1].Talents)
            {
                var reward = Instantiate(this.rewardPrefab, this.rewardContainer);
                reward.Construct(talent);
                this.rewards.Add(reward);
            }
        }

        private void OnCompleteButtonPointerClick()
        {
            CompleteButtonClicked?.Invoke();
        }
    }
}