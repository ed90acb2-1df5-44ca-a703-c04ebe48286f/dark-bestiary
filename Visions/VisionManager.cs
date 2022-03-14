using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.Skills;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;
using Object = UnityEngine.Object;

namespace DarkBestiary.Visions
{
    public class VisionManager
    {
        public static event Payload Completed;

        public static VisionManager Instance { get; private set; }
        public static bool IsNewGame { get; set; } = true;

        public event Payload<float> SanityChanged;

        private const int MaxActCount = 3;
        public const float InitialSanity = 800 * MaxActCount;

        public int CurrentAct { get; private set; } = 1;
        public bool IsVictory { get; private set; }
        public bool IsDefeat { get; private set; }
        public bool IsVictoryOrDefeat => IsVictory || IsDefeat;
        public List<SkillSlot> SkillSlots { get; private set; }

        public float Sanity
        {
            get => this.sanity;
            set
            {
                this.sanity = Mathf.Max(0, value);
                SanityChanged?.Invoke(this.sanity);
            }
        }

        private readonly CharacterManager characterManager;
        private readonly VisionProgression progression;
        private readonly ISkillRepository skillRepository;
        private readonly IBehaviourRepository behaviourRepository;
        private readonly IFileReader fileReader;
        private readonly SummaryRecorder summaryRecorder;
        private readonly VisionRunner visionRunner;
        private readonly List<Behaviour> talentBehaviours = new List<Behaviour>();

        private float sanity;
        private VisionView recentlyCompleted;
        private VisionView pendingCompletion;
        private VisionMap map;
        private VisionsSummaryViewController summaryViewController;
        private VisionProgressionViewController progressionViewController;
        private VisionMapViewController mapViewController;

        public VisionManager(
            CharacterManager characterManager,
            VisionProgression progression,
            IScenarioRepository scenarioRepository,
            ICurrencyRepository currencyRepository,
            ILootDataRepository lootRepository,
            IItemRepository itemRepository,
            ISkillRepository skillRepository,
            IBehaviourRepository behaviourRepository,
            IFileReader fileReader,
            IItemCategoryRepository itemCategoryRepository)
        {
            this.characterManager = characterManager;
            this.progression = progression;
            this.skillRepository = skillRepository;
            this.behaviourRepository = behaviourRepository;
            this.fileReader = fileReader;
            this.summaryRecorder = new SummaryRecorder();
            this.visionRunner = new VisionRunner(this,
                characterManager,
                scenarioRepository,
                currencyRepository,
                lootRepository,
                itemRepository,
                behaviourRepository,
                itemCategoryRepository);
        }

        public static bool IsSaveExists()
        {
            return Container.Instance.Resolve<IFileReader>().Read<VisionMapSaveData>(GetDataPath())?.Visions.Count > 0;
        }

        public int GetRewardCount()
        {
            return Mathf.Max(1, Mathf.FloorToInt(Sanity / 400));
        }

        public int GetVisionStrength()
        {
            if (Sanity <= 0)
            {
                return 100;
            }

            return Mathf.FloorToInt((InitialSanity - Sanity) / 50);
        }

        public static void MaybeInitialize()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = Container.Instance.Instantiate<VisionManager>();
            Instance.Initialize();
        }

        public void Initialize()
        {
            IsVictory = false;
            IsDefeat = false;
            Sanity = InitialSanity;
            SkillSlots = new List<SkillSlot>();

            var skills = this.skillRepository.Find(s => s.IsEnabled && s.Flags.HasFlag(SkillFlags.Vision));

            for (var i = 0; i < skills.Count; i++)
            {
                skills[i].Caster = this.characterManager.Character.Entity;
                var skillSlot = new SkillSlot(i, SkillType.Common);
                skillSlot.ChangeSkill(skills[i]);
                SkillSlots.Add(skillSlot);
            }

            this.characterManager.Character.Entity.transform.position = new Vector3(-500, 0, 0);

            this.visionRunner.Completed += OnVisionCompleted;
            this.visionRunner.Failed += OnVisionFailed;
            this.visionRunner.Initialize();

            this.mapViewController = Container.Instance.Instantiate<VisionMapViewController>();
            this.mapViewController.View.AnySkillClicked += OnSkillClicked;
            this.mapViewController.Initialize();

            this.map = Object.Instantiate(Resources.Load<VisionMap>("Prefabs/VisionMap"));
            this.map.AnyVisionClicked += OnAnyVisionClicked;
            this.map.Completed += OnMapCompleted;
            this.map.Initialize();

            if (IsNewGame)
            {
                ApplyLearnedTalents();
                MaybeWithdrawIllusoryItem();
                this.map.Generate(CurrentAct);
                this.summaryRecorder.Start();
            }
            else
            {
                try
                {
                    var save = this.fileReader.Read<VisionMapSaveData>(GetDataPath());

                    LoadMap(save);

                    LoadLearnedTalents(save);
                    LoadTemporaryBehaviours(save);
                    this.summaryRecorder.Start(save.Summary);
                }
                catch (Exception exception)
                {
                    UiErrorFrame.Instance.ShowMessage("Invalid vision map data version.");
                    this.map.Generate(CurrentAct);
                    throw;
                }
            }

            this.progression.Talents.UnlearnAll();
            this.progression.Talents.Points = this.progression.Experience.Level;

            GameState.AnyGameStateEnter += OnAnyGameStateEnter;
            Application.quitting += OnApplicationQuitting;
        }

        private void Terminate()
        {
            if (IsVictory || IsDefeat)
            {
                Save(new VisionMapSaveData());
            }
            else
            {
                Save();
            }

            // Note: to prevent destruction along with the map.
            this.characterManager.Character.Entity.transform.SetParent(null);

            this.progressionViewController?.Terminate();

            this.mapViewController.View.AnySkillClicked -= OnSkillClicked;
            this.mapViewController.Terminate();

            this.summaryRecorder.Stop();

            this.visionRunner.Completed -= OnVisionCompleted;
            this.visionRunner.Failed -= OnVisionFailed;
            this.visionRunner.Terminate();

            this.map.AnyVisionClicked -= OnAnyVisionClicked;
            this.map.Completed -= OnMapCompleted;
            this.map.Terminate();

            GameState.AnyGameStateEnter -= OnAnyGameStateEnter;
            Application.quitting -= OnApplicationQuitting;
        }

        public int GetVisionMonsterLevel(VisionData vision, int actIndex)
        {
            // Act 1 = 00-10 (15)
            // Act 2 = 10-20 (25)
            // Act 3 = 20-30 (35)

            var levelMin = (actIndex - 1) * 10;
            var levelMax = levelMin + 10;

            if (vision.IsFinal)
            {
                levelMin = levelMax + 5;
                levelMax = levelMin;
            }

            var characterLevel = this.characterManager.Character.Entity.GetComponent<ExperienceComponent>().Experience.Level;

            return Mathf.Clamp(characterLevel, levelMin, levelMax);
        }

        public void LoadTemporaryBehaviours(VisionMapSaveData data)
        {
            var character = this.characterManager.Character.Entity;
            var behavioursComponent = character.GetComponent<BehavioursComponent>();

            foreach (var behaviourSaveData in data.TemporaryBehaviours)
            {
                var behaviour = this.behaviourRepository.Find(behaviourSaveData.BehaviourId);
                behaviour.StackCount = behaviourSaveData.StackCount;
                behavioursComponent.ApplyAllStacks(behaviour, character);
                behaviour.RemainingDuration = behaviourSaveData.RemainingDuration;
            }
        }

        public void LoadLearnedTalents(VisionMapSaveData data)
        {
            var behaviours = this.behaviourRepository.Find(data.TalentBehaviours).Where(b => !b.IsOneshot).ToList();
            ApplyAndStoreTalentBehaviours(behaviours);
        }

        public void ApplyLearnedTalents()
        {
            var behaviours = new List<Behaviour>();

            foreach (var tier in this.progression.Talents.Tiers)
            {
                foreach (var talent in tier.Talents)
                {
                    if (!talent.IsLearned)
                    {
                        continue;
                    }

                    behaviours.Add(talent.Behaviour);
                }
            }

            ApplyAndStoreTalentBehaviours(behaviours);
        }

        private void ApplyAndStoreTalentBehaviours(List<Behaviour> behaviours)
        {
            var character = this.characterManager.Character.Entity;
            var behavioursComponent = character.GetComponent<BehavioursComponent>();

            foreach (var behaviour in behaviours)
            {
                behavioursComponent.ApplyStack(behaviour, character);
            }

            this.talentBehaviours.Clear();
            this.talentBehaviours.AddRange(behaviours);
        }

        private void OnApplicationQuitting()
        {
            Terminate();
        }

        private void MaybeWithdrawIllusoryItem()
        {
            var illusoryItem = Stash.Instance.Inventories.SelectMany(inventory => inventory.Items.Where(i => i.IsMarkedAsIllusory)).Random();

            if (illusoryItem == null)
            {
                return;
            }

            illusoryItem.Inventory.Remove(illusoryItem);
            this.characterManager.Character.Entity.GetComponent<InventoryComponent>().Pickup(illusoryItem);
        }

        public void RevealMap(int range)
        {
            var center = this.recentlyCompleted;

            if (center == null)
            {
                center = this.map.VisionViews.FirstOrDefault(v => v.IsUnlocked);

                if (center == null)
                {
                    return;
                }
            }

            foreach (var visionView in this.map.WithinRange(center.transform.position, range))
            {
                if (visionView.IsCompletedOrSkipped || visionView.IsUnlocked)
                {
                    continue;
                }

                visionView.Reveal();
            }
        }

        public void CompleteRandomVision()
        {
            var random = this.map.VisionViews.Where(v => v.IsLocked).Random();

            if (random == null)
            {
                return;
            }

            this.map.ScrollTo(random.transform, () => {OnVisionCompleted(random);});
        }

        private void OnSkillClicked(SkillSlotView skillView)
        {
            if (!this.map.gameObject.activeSelf || this.map.IsLocked)
            {
                return;
            }

            try
            {
                skillView.Slot.Skill.Use(Vector3.zero);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnVictory()
        {
            IsVictory = true;
        }

        private void OnDefeat()
        {
            IsDefeat = true;
        }

        private void OnMapCompleted()
        {
            if (CurrentAct >= MaxActCount)
            {
                OnVictory();
                return;
            }

            CurrentAct++;

            this.characterManager.Character.Entity.transform.position = new Vector3(-500, 0, 0);

            Timer.Instance.Wait(2, () => { this.map.Generate(CurrentAct); });
        }

        private void OnAnyVisionClicked(VisionView vision)
        {
            if (this.visionRunner.IsRunning)
            {
                return;
            }

            Save();

            this.visionRunner.Run(vision);
        }

        private void OnVisionCompleted(VisionView vision)
        {
            this.recentlyCompleted = vision;

            foreach (var skillSlot in SkillSlots)
            {
                skillSlot.Skill.ReduceCooldown(1);
            }

            if (!this.map.gameObject.activeSelf)
            {
                this.pendingCompletion = vision;
                return;
            }

            CompleteVision(vision);
        }

        private void CompleteVision(VisionView vision)
        {
            Sanity -= vision.Data.Sanity;

            if (!vision.Data.IsFinal)
            {
                this.characterManager.Character.Entity.transform.position = vision.transform.position;
            }

            this.map.OnVisionCompleted(vision);
        }

        private void OnVisionFailed(VisionView vision)
        {
            OnDefeat();
        }

        private void OnAnyGameStateEnter(GameState state)
        {
            if (state.IsMainMenu)
            {
                OnEnterMainMenu();
                return;
            }

            this.characterManager.Character.Entity.transform.SetParent(null);
            this.characterManager.Character.Entity.transform.localScale = Vector3.one;

            this.map.gameObject.SetActive(false);
            this.mapViewController.View.Hide();

            if(state.IsVisionsMap)
            {
                OnEnterVisionMap();
            }
        }

        private static void OnEnterMainMenu()
        {
            Instance.Terminate();
            Instance = null;
        }

        private void OnEnterVisionMap()
        {
            MusicManager.Instance.Play("event:/Music/Visions");

            this.characterManager.Character.Entity.transform.SetParent(this.map.VisionContainer, false);
            this.map.gameObject.SetActive(true);
            this.mapViewController.View.Show();

            if (this.pendingCompletion != null)
            {
                CompleteVision(this.pendingCompletion);
                this.pendingCompletion = null;
            }

            if (IsDefeat || IsVictory)
            {
                if (IsVictory)
                {
                    Completed?.Invoke();
                }

                this.map.IsLocked = true;
                Timer.Instance.Wait(IsVictory ? 2.0f : 0, () => { ShowSummary(IsVictory); });
            }
        }

        private void ShowSummary(bool isSuccess)
        {
            this.summaryViewController = ViewControllerRegistry.Initialize<VisionsSummaryViewController>(new object[]{this.summaryRecorder.GetResult(), isSuccess});
            this.summaryViewController.Terminated += OnSummaryViewControllerTerminated;
            this.summaryViewController.View.Show();
        }

        private void OnSummaryViewControllerTerminated(IViewController<IVisionSummaryView> controller)
        {
            this.summaryViewController.Terminated -= OnSummaryViewControllerTerminated;

            this.progressionViewController = ViewControllerRegistry.Initialize<VisionProgressionViewController>();
            this.progressionViewController.View.Show();
        }

        private void Save()
        {
            Save(GetSaveData());
        }

        private void Save(VisionMapSaveData save)
        {
            this.fileReader.Write(save, GetDataPath());
        }

        private VisionMapSaveData GetSaveData()
        {
            var save = new VisionMapSaveData();

            var temporaryBehavioursData = this.characterManager.Character.Entity
                .GetComponent<BehavioursComponent>()
                .Behaviours
                .Where(b => b.Duration >= 1)
                .Select(b => new VisionBehaviourSaveData
                {
                    BehaviourId = b.Id,
                    RemainingDuration = b.RemainingDuration,
                    StackCount = b.StackCount
                })
                .ToList();

            save.TalentBehaviours = this.talentBehaviours.Select(b => b.Id).ToList();
            save.TemporaryBehaviours = temporaryBehavioursData;
            save.CurrentAct = CurrentAct;
            save.Sanity = Sanity;
            save.LastCompletedVisionIndex = this.recentlyCompleted?.Index ?? -1;
            save.FinalVisionData = this.map.FinalVision.Data;
            save.Character = Container.Instance.Resolve<CharacterMapper>().ToData(this.characterManager.Character);
            save.Summary = this.summaryRecorder.GetResult();

            foreach (var skillSlot in SkillSlots)
            {
                save.Skills.Add(new VisionSkillSaveData {SkillId = skillSlot.Skill.Id, Cooldown = skillSlot.Skill.RemainingCooldown});
            }

            foreach (var visionView in this.map.VisionViews)
            {
                save.Visions.Add(new VisionSaveData {VisionData = visionView.Data, VisionState = visionView.State});
            }

            return save;
        }

        public static void LoadCharacter()
        {
            var save = Container.Instance.Resolve<IFileReader>().Read<VisionMapSaveData>(GetDataPath());
            CharacterManager.Instance.Select(Container.Instance.Resolve<CharacterMapper>().ToEntity(save.Character));
        }

        private void LoadMap(VisionMapSaveData save)
        {
            CurrentAct = save.CurrentAct;
            Sanity = save.Sanity;

            foreach (var skillSlot in SkillSlots)
            {
                var visionSkillSaveData = save.Skills.FirstOrDefault(s => s.SkillId == skillSlot.Skill.Id);

                if (visionSkillSaveData == null || visionSkillSaveData.Cooldown == 0)
                {
                    continue;
                }

                skillSlot.Skill.RunCooldown(visionSkillSaveData.Cooldown);
            }

            var visions = save.Visions.Select(data => data.VisionData).ToList();

            this.map.CreateVisionViews(save.CurrentAct, visions);
            this.map.CreateFinalVision(save.FinalVisionData, save.CurrentAct);

            if (save.LastCompletedVisionIndex != -1)
            {
                this.recentlyCompleted = this.map.VisionViews[save.LastCompletedVisionIndex];
                this.map.SetCurrentVision(this.recentlyCompleted);
                this.map.ScrollTo(this.recentlyCompleted.transform);

                Timer.Instance.WaitForFixedUpdate(() =>
                {
                    this.characterManager.Character.Entity.transform.position = this.recentlyCompleted.transform.position;
                });
            }

            this.map.FinalVision.Unlock();

            for (var i = 0; i < save.Visions.Count; i++)
            {
                switch (save.Visions[i].VisionState)
                {
                    case VisionViewState.Locked:
                        this.map.VisionViews[i].Lock();
                        break;
                    case VisionViewState.Unlocked:
                        this.map.VisionViews[i].Unlock();
                        break;
                    case VisionViewState.Completed:
                        this.map.VisionViews[i].Complete();
                        break;
                    case VisionViewState.Skipped:
                        this.map.VisionViews[i].Skip();
                        break;
                    case VisionViewState.Revealed:
                        this.map.VisionViews[i].Reveal();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static string GetDataPath()
        {
            return Environment.PersistentDataPath + $"/{Container.Instance.Resolve<StorageId>()}/visions.save";
        }
    }
}