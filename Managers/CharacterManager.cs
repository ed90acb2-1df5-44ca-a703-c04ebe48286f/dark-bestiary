using System;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Managers
{
    public class CharacterManager : IInitializable
    {
        public static event Payload<Character> CharacterSelected;
        public static event Payload<UnitComponent> BestiaryUpdated;

        public static CharacterManager Instance { get; private set; }

        public Character Character { get; private set; }

        private readonly ICharacterRepository characterRepository;

        public CharacterManager(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public void Initialize()
        {
            Instance = this;

            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            UnitComponent.AnyUnitTerminated += OnAnyComponentTerminated;
            GameState.AnyGameStateEnter += OnAnyGameStateEnter;
            GameState.AnyGameStateExit += OnAnyGameStateExit;
            Scenario.AnyScenarioCompleted += OnAnyScenarioCompleted;
            Application.quitting += Save;
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            if (!data.Killer.IsAllyOfPlayer() ||
                data.Victim.IsDummy() ||
                data.Victim.IsSummoned() ||
                data.Victim.IsAllyOfPlayer())
            {
                return;
            }

            var unit = data.Victim.GetComponent<UnitComponent>();

            if (Character.Data.UnlockedMonsters.Any(unitId => unit.Id == unitId))
            {
                return;
            }

            Character.Data.UnlockedMonsters.Add(unit.Id);

            BestiaryUpdated?.Invoke(unit);
        }

        private void OnAnyComponentTerminated(UnitComponent unit)
        {
            if (Character == null || Character.Entity != unit.gameObject)
            {
                return;
            }

            Character.Entity.GetComponent<ExperienceComponent>().Experience.LevelUp -= OnLevelUp;
            Character = null;
        }

        public void Select(Character character)
        {
            Character = character;
            Character.Data.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Character.Entity.GetComponent<ExperienceComponent>().Experience.LevelUp += OnLevelUp;

            CharacterSelected?.Invoke(character);
        }

        private void OnLevelUp(Experience experience)
        {
            Character.Data.Rerolls++;
        }

        private void OnAnyGameStateEnter(GameState gameState)
        {
            if (gameState.IsMainMenu)
            {
                Character?.Entity.Terminate();
            }
        }

        private void OnAnyGameStateExit(GameState gameState)
        {
            Save();
        }

        private void OnAnyScenarioCompleted(Scenario scenario)
        {
            if (scenario.IsStart)
            {
                Character.IsStartScenarioCompleted = true;
            }

            if (!scenario.IsDisposable)
            {
                Character.CompletedScenarios.Add(scenario.Id);
            }

            Character.AvailableScenarios.Remove(scenario.Id);
            Character.AvailableScenarios.AddRange(
                scenario.Children.Where(scenarioId => !Character.CompletedScenarios.Contains(scenarioId)));
        }

        private void Save()
        {
            if (Character == null)
            {
                return;
            }

            this.characterRepository.Save(Character);
        }
    }
}