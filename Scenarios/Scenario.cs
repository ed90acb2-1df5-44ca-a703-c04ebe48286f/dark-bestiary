using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.Scenarios
{
    public class Scenario
    {
        public static event Payload<Scenario> AnyScenarioStarted;
        public static event Payload<Scenario> AnyScenarioFailed;
        public static event Payload<Scenario> AnyScenarioCompleted;
        public static event Payload<Scenario> AnyScenarioExit;

        public static Scenario Active { get; private set; }

        public int Id { get; }
        public int Index { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public I18NString CompleteText { get; }
        public I18NString Commentary { get; }
        public float PositionX { get; }
        public float PositionY { get; }
        public bool IsStart { get; }
        public bool IsEnd { get; }
        public bool IsDisposable { get; }
        public bool IsFailed { get; private set; }
        public List<Item> GuaranteedRewards { get; }
        public List<Item> ChoosableRewards { get; }
        public List<Episode> Episodes { get; }
        public List<int> Children { get; }
        public bool IsActiveEpisodeLast => ActiveEpisodeIndex == Episodes.Count - 1;
        public int ActiveEpisodeIndex => Episodes.IndexOf(this.activeEpisode);

        public Episode NextEpisode => Episodes.IndexInBounds(ActiveEpisodeIndex + 1)
            ? Episodes[ActiveEpisodeIndex + 1] : null;

        public Episode PreviousEpisode { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime StoppedAt { get; private set; }

        private readonly CharacterManager characterManager;

        private ScenarioSummaryRecorder summaryRecorder;
        private ScenarioLootRecorder lootRecorder;

        private GameObject weather;
        private Episode activeEpisode;
        private Item chosenReward;

        public Scenario(ScenarioData data, List<Episode> episodes,
            List<Item> guaranteedRewards, List<Item> choosableRewards, CharacterManager characterManager)
        {
            this.characterManager = characterManager;

            Id = data.Id;
            Index = data.Index;
            PositionX = data.PositionX;
            PositionY = data.PositionY;
            IsDisposable = data.IsDisposable;
            IsStart = data.IsStart;
            IsEnd = data.IsEnd;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Commentary = I18N.Instance.Get(data.CommentaryKey);
            CompleteText = I18N.Instance.Get(data.CompleteKey);
            Children = data.Children;
            GuaranteedRewards = guaranteedRewards;
            ChoosableRewards = choosableRewards;
            Episodes = episodes;
        }

        public void Initialize()
        {
            StartedAt = DateTime.Now;

            Active = this;
            StartNextEpisode();

            this.summaryRecorder = new ScenarioSummaryRecorder();
            this.summaryRecorder.Start();

            this.lootRecorder = new ScenarioLootRecorder();
            this.lootRecorder.Start();

            AnyScenarioStarted?.Invoke(this);
        }

        public void Terminate()
        {
            MusicManager.Instance.Stop();

            AnyScenarioExit?.Invoke(this);

            foreach (var episode in Episodes)
            {
                if (episode == this.activeEpisode)
                {
                    episode.Stop();
                }

                episode.Terminate();
            }

            OnTerminated();

            Active = null;
        }

        public ScenarioSummary GetSummary()
        {
            return this.summaryRecorder.GetResult();
        }

        public ScenarioLoot GetLoot()
        {
            return this.lootRecorder.GetResult();
        }

        public void Tick(float delta)
        {
            this.activeEpisode?.Tick(delta);
        }

        public void StartNextEpisode()
        {
            if (IsActiveEpisodeLast)
            {
                return;
            }

            ScreenFade.Instance.To(() => SwitchEpisode(Episodes[ActiveEpisodeIndex + 1]));
        }

        public void StartPreviousEpisode()
        {
            if (ActiveEpisodeIndex == 0)
            {
                Debug.LogWarning("Cannot start previous episode. Active episode is first.");
                return;
            }

            ScreenFade.Instance.To(() => SwitchEpisode(Episodes[ActiveEpisodeIndex - 1]));
        }

        public void ClaimRewards()
        {
            if (ChoosableRewards.Count > 0 && this.chosenReward == null)
            {
                throw new MustChooseRewardException();
            }

            var inventory = this.characterManager.Character.Entity.GetComponent<InventoryComponent>();
            inventory.Pickup(GuaranteedRewards);

            if (this.chosenReward != null)
            {
                inventory.Pickup(this.chosenReward);
            }
        }

        public void ChooseReward(Item item)
        {
            this.chosenReward = item;
        }

        private void SwitchEpisode(Episode episode)
        {
            if (this.activeEpisode != null)
            {
                PreviousEpisode = this.activeEpisode;
                this.activeEpisode.Completed -= OnEpisodeCompleted;
                this.activeEpisode.Failed -= OnEpisodeFailed;
                this.activeEpisode.Stop();
            }

            this.activeEpisode = episode;

            if (PreviousEpisode != null)
            {
                var allies = PreviousEpisode.Scene.Entities
                    .AliveInTeam(1).Where(e => !e.IsCharacter() && !e.IsImmovable()).ToList();

                PreviousEpisode.Scene.Entities.Remove(allies);
                this.activeEpisode.Scene.Entities.Add(allies);
            }

            if (!this.activeEpisode.IsCompleted)
            {
                this.activeEpisode.Completed += OnEpisodeCompleted;
                this.activeEpisode.Failed += OnEpisodeFailed;
                this.activeEpisode.Initialize(this);
            }

            MusicManager.Instance.Play(this.activeEpisode.Scene.Data.Environment.Ambience);
            this.activeEpisode.Start();
        }

        private void GiveLootAndExperience()
        {
            this.summaryRecorder.Stop();
            this.lootRecorder.Stop();

            var character = this.characterManager.Character.Entity;
            var loot = this.lootRecorder.GetResult();

            var experienceMultiplier = character.GetComponent<PropertiesComponent>()
                .Get(PropertyType.Experience).Value();

            var experienceGain = (int) (loot.Experience.Sum() * experienceMultiplier);

            var experience = character.GetComponent<ExperienceComponent>();
            experience.GiveExperience(experienceGain);

            var reliquary = character.GetComponent<ReliquaryComponent>();
            reliquary.GiveExperience(experienceGain);
        }

        private void OnEpisodeCompleted(Episode episode)
        {
            this.activeEpisode.Completed -= OnEpisodeCompleted;

            if (Episodes.All(item => item.IsCompleted) && this.characterManager.Character.Entity.IsAlive())
            {
                OnCompleted();
                AnyScenarioCompleted?.Invoke(this);
            }
        }

        private void OnEpisodeFailed(Episode episode)
        {
            IsFailed = true;
            OnFailed();
            AnyScenarioFailed?.Invoke(this);
        }

        private void OnTerminated()
        {
            var loot = this.lootRecorder.GetResult();
            this.characterManager.Character.Entity.GetComponent<InventoryComponent>().Pickup(loot.Items);
        }

        private void OnCompleted()
        {
            StoppedAt = DateTime.Now;
            GiveLootAndExperience();
        }

        private void OnFailed()
        {
            StoppedAt = DateTime.Now;
            GiveLootAndExperience();
        }
    }
}