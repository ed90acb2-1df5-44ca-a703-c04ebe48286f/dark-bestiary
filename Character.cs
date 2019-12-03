using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Dialogues;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary
{
    public class Character
    {
        public static event Payload<int> ScenarioUnlocked;
        public static event Payload<Recipe> RecipeUnlocked;

        public int Id { get; }
        public string Name { get; }
        public GameObject Entity { get; set; }
        public bool IsStartScenarioCompleted { get; set; }
        public List<int> AvailableScenarios { get; }
        public List<int> CompletedScenarios { get; }
        public CharacterData Data { get; }
        public List<Relic> Relics { get; }

        public Character(CharacterData data, GameObject entity)
        {
            Id = data.Id;
            Name = data.Name;
            Data = data;
            Entity = entity;
            IsStartScenarioCompleted = data.IsStartScenarioCompleted;
            AvailableScenarios = data.AvailableScenarios;
            CompletedScenarios = data.CompletedScenarios;
            Relics = entity.GetComponent<ReliquaryComponent>().Available;
        }

        public bool IsDialogueRead(Dialogue dialogue)
        {
            return Data.ReadDialogues.Contains(dialogue.Id);
        }

        public void UnlockScenario(int scenarioId)
        {
            if (AvailableScenarios.Contains(scenarioId))
            {
                return;
            }

            AvailableScenarios.Add(scenarioId);
            ScenarioUnlocked?.Invoke(scenarioId);
        }

        public void UnlockRecipe(Recipe recipe)
        {
            if (Data.UnlockedRecipes.Contains(recipe.Id))
            {
                return;
            }

            Data.UnlockedRecipes.Add(recipe.Id);
            RecipeUnlocked?.Invoke(recipe);
        }
    }
}