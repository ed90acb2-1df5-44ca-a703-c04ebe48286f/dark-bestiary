using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Interaction;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Properties;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.Scenarios
{
    public class Scenario
    {
        public static event Action<Scenario> AnyScenarioStarted;
        public static event Action<Scenario> AnyScenarioFailed;
        public static event Action<Scenario> AnyScenarioCompleted;
        public static event Action<Scenario> AnyScenarioExit;

        public static Scenario Active { get; private set; }

        public int Id { get; }
        public int Index { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public I18NString CompleteText { get; }
        public I18NString Commentary { get; }
        public float PositionX { get; }
        public float PositionY { get; }
        public bool IsUnlocked { get; }
        public bool IsStart { get; }
        public bool IsEnd { get; }
        public bool IsDisposable { get; }
        public bool IsAscension { get; }
        public bool IsDepths { get; }
        public bool IsFailed { get; private set; }
        public List<Item> GuaranteedRewards { get; }
        public List<Item> ChoosableRewards { get; set; }
        public List<Episode> Episodes { get; }
        public List<int> Children { get; }
        public bool IsActiveEpisodeLast => ActiveEpisodeIndex == Episodes.Count - 1;
        public int ActiveEpisodeIndex => Episodes.IndexOf(m_ActiveEpisode);

        public DateTime StartedAt { get; private set; }
        public DateTime StoppedAt { get; private set; }

        private SummaryRecorder m_SummaryRecorder;
        private DeathRecapRecorder m_DeathRecapRecorder;
        private ScenarioLootRecorder m_LootRecorder;

        private GameObject m_Weather;
        private Episode m_ActiveEpisode;
        private Item m_ChosenReward;

        public Scenario(ScenarioData data, List<Episode> episodes, List<Item> guaranteedRewards, List<Item> choosableRewards)
        {
            Id = data.Id;
            Index = data.Index;
            PositionX = data.PositionX;
            PositionY = data.PositionY;
            IsUnlocked = data.IsUnlocked;
            IsDisposable = data.IsDisposable;
            IsAscension = data.IsAscension;
            IsDepths = data.IsDepths;
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

            m_SummaryRecorder = new SummaryRecorder();
            m_SummaryRecorder.Start();

            m_DeathRecapRecorder = new DeathRecapRecorder();
            m_DeathRecapRecorder.Start();

            m_LootRecorder = new ScenarioLootRecorder(this);
            m_LootRecorder.Start();

            AnyScenarioStarted?.Invoke(this);
        }

        public void AddExperience(int experience)
        {
            m_LootRecorder.AddExperience(experience);
        }

        public void AddLoot(IEnumerable<Item> items)
        {
            m_LootRecorder.AddItems(items);
        }

        public void Terminate()
        {
            MusicManager.Instance.Stop();
            m_SummaryRecorder.Stop();
            m_DeathRecapRecorder.Stop();
            m_LootRecorder.Stop();

            AnyScenarioExit?.Invoke(this);

            foreach (var episode in Episodes)
            {
                if (episode == m_ActiveEpisode)
                {
                    episode.Stop();
                }

                episode.Terminate();
            }

            OnTerminated();

            Active = null;
        }

        public Summary GetSummary()
        {
            return m_SummaryRecorder.GetResult();
        }

        public DeathRecapRecorder GetDeathRecap()
        {
            return m_DeathRecapRecorder;
        }

        public ScenarioLoot GetLoot()
        {
            return m_LootRecorder.GetResult();
        }

        public void Tick(float delta)
        {
            m_ActiveEpisode?.Tick(delta);
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
            if (ChoosableRewards.Count > 0 && m_ChosenReward == null)
            {
                throw new MustChooseRewardException();
            }

            var inventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
            inventory.Pickup(GuaranteedRewards);

            if (m_ChosenReward != null)
            {
                inventory.Pickup(m_ChosenReward);
            }
        }

        public void ChooseReward(Item item)
        {
            m_ChosenReward = item;
        }

        private void SwitchEpisode(Episode episode)
        {
            if (m_ActiveEpisode != null)
            {
                m_ActiveEpisode.Completed -= OnEpisodeCompleted;
                m_ActiveEpisode.Failed -= OnEpisodeFailed;
                m_ActiveEpisode.Stop();
            }

            m_ActiveEpisode = episode;
            m_ActiveEpisode.Completed += OnEpisodeCompleted;
            m_ActiveEpisode.Failed += OnEpisodeFailed;
            m_ActiveEpisode.Initialize(this);
            m_ActiveEpisode.Start();
        }

        public void GiveExperience()
        {
            var character = Game.Instance.Character.Entity;
            var loot = m_LootRecorder.GetResult();

            var experienceMultiplier = character.GetComponent<PropertiesComponent>()
                .Get(PropertyType.Experience).Value();

            var experienceGain = (int) (loot.Experience * experienceMultiplier);

            var experience = character.GetComponent<ExperienceComponent>();
            experience.GiveExperience(experienceGain);

            var reliquary = character.GetComponent<ReliquaryComponent>();
            reliquary.GiveExperience(experienceGain);
        }

        private void OnEpisodeCompleted(Episode episode)
        {
            m_ActiveEpisode.Completed -= OnEpisodeCompleted;

            if (IsActiveEpisodeLast && Game.Instance.Character.Entity.IsAlive())
            {
                OnCompleted();
                AnyScenarioCompleted?.Invoke(this);
            }
            else
            {
                Timer.Instance.Wait(2, StartNextEpisode);
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
            var loot = m_LootRecorder.GetResult();
            Game.Instance.Character.Entity.GetComponent<InventoryComponent>().Pickup(loot.Items);
        }

        private void OnCompleted()
        {
            Interactor.Instance.EnterSelectionState();
            StoppedAt = DateTime.Now;
            GiveExperience();
        }

        private void OnFailed()
        {
            Interactor.Instance.EnterSelectionState();
            StoppedAt = DateTime.Now;
            GiveExperience();
        }
    }
}