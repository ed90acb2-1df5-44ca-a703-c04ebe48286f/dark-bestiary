using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public struct ScenarioInfo
    {
        public readonly ScenarioData Data;
        public readonly List<Item> Rewards;
        public readonly List<Behaviour> MonsterModifiers;
        public readonly ScenarioStatus Status;

        public ScenarioInfo(ScenarioData data, List<Item> rewards, List<Behaviour> monsterModifiers,
            ScenarioStatus status)
        {
            this.Data = data;
            this.Rewards = rewards;
            this.MonsterModifiers = monsterModifiers;
            this.Status = status;
        }
    }

    public class CommandBoardViewController : ViewController<ICommandBoardView>
    {
        private readonly IScenarioDataRepository scenarioDataRepository;
        private readonly IBehaviourRepository behaviourRepository;
        private readonly IItemRepository itemRepository;
        private readonly IUnitDataRepository unitDataRepository;
        private readonly SpellbookComponent spellbook;
        private readonly CharacterManager characterManager;

        public CommandBoardViewController(ICommandBoardView view, CharacterManager characterManager,
            IScenarioDataRepository scenarioDataRepository, IBehaviourRepository behaviourRepository,
            IItemRepository itemRepository, IUnitDataRepository unitDataRepository) : base(view)
        {
            this.scenarioDataRepository = scenarioDataRepository;
            this.behaviourRepository = behaviourRepository;
            this.itemRepository = itemRepository;
            this.unitDataRepository = unitDataRepository;
            this.characterManager = characterManager;
            this.spellbook = this.characterManager.Character.Entity.GetComponent<SpellbookComponent>();
        }

        protected override void OnInitialize()
        {
            var scenarios = this.scenarioDataRepository.FindAll()
                .Where(scenario => !scenario.IsHidden &&
                                   !(DetermineStatus(scenario) == ScenarioStatus.Unavailable && scenario.IsDisposable))
                .OrderBy(scenario => scenario.Index)
                .Select(Wrap)
                .ToList();

            Character.ScenarioUnlocked += OnScenarioUnlocked;

            View.PlaceOnActionBar += OnPlaceOnActionBar;
            View.Replace += OnSkillReplace;
            View.ScenarioStart += OnStartScenario;
            View.Construct(scenarios);
        }

        protected override void OnTerminate()
        {
            Character.ScenarioUnlocked -= OnScenarioUnlocked;

            View.PlaceOnActionBar -= OnPlaceOnActionBar;
            View.Replace -= OnSkillReplace;
            View.ScenarioStart -= OnStartScenario;
        }

        private void OnScenarioUnlocked(int scenarioId)
        {
            View.AddScenario(Wrap(this.scenarioDataRepository.Find(scenarioId)));
        }

        private ScenarioInfo Wrap(ScenarioData data)
        {
            return new ScenarioInfo(data, GetRewards(data), GetModifiers(data), DetermineStatus(data));
        }

        private List<Item> GetRewards(ScenarioData data)
        {
            var rewards = new List<Item>();

            if (this.characterManager.Character.CompletedScenarios.Contains(data.Id) && data.Type == ScenarioType.Campaign)
            {
                return rewards;
            }

            foreach (var rewardData in data.Rewards)
            {
                var item = this.itemRepository.Find(rewardData.ItemId);
                item.ChangeOwner(this.characterManager.Character.Entity);
                item.SetStack(rewardData.StackCount);
                rewards.Add(item);
            }

            return rewards;
        }

        private List<Behaviour> GetModifiers(ScenarioData data)
        {
            var behaviours = new List<Behaviour>();

            foreach (var episodeData in data.Episodes)
            {
                foreach (var unitInfo in episodeData.Encounter.UnitTable.Units)
                {
                    var unitData = this.unitDataRepository.Find(unitInfo.UnitId);

                    foreach (var behaviour in this.behaviourRepository.Find(unitData.Behaviours))
                    {
                        if (!behaviour.IsMonsterModifier)
                        {
                            continue;
                        }

                        behaviours.Add(behaviour);
                    }
                }
            }

            return behaviours.DistinctBy(b => b.Id).ToList();
        }

        private ScenarioStatus DetermineStatus(ScenarioData scenario)
        {
            if (this.characterManager.Character.CompletedScenarios.Contains(scenario.Id))
            {
                return ScenarioStatus.Completed;
            }

            if (this.characterManager.Character.AvailableScenarios.Contains(scenario.Id))
            {
                return ScenarioStatus.Available;
            }

            return ScenarioStatus.Unavailable;
        }

        private void OnSkillReplace(Skill skillA, Skill skillB)
        {
            this.spellbook.Replace(skillA, skillB);

            if (CombatEncounter.Active != null)
            {
                skillB.RunCooldown();
            }
        }

        private void OnPlaceOnActionBar(SkillSlot slot, Skill skill)
        {
            try
            {
                this.spellbook.PlaceOnActionBar(slot, skill);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }

        private void OnStartScenario(ScenarioInfo info)
        {
            Game.Instance.SwitchState(
                new ScenarioGameState(
                    Container.Instance.Instantiate<ScenarioMapper>().ToEntity(info.Data),
                    this.characterManager.Character
                )
            );
        }
    }
}