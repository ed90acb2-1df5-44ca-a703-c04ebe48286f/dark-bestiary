using System;
using DarkBestiary.Components;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class LevelupConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;

        public LevelupConsoleCommand(CharacterManager characterManager)
        {
            this.characterManager = characterManager;
        }

        public string GetSignature()
        {
            return "levelup";
        }

        public string GetDescription()
        {
            return "Level Up!. (Format: [levels]";
        }

        public string Execute(string input)
        {
            var strings = input.Split();
            var levels = strings.Length > 0 ? Math.Max(int.Parse(strings[0]), 1) : 1;
            var experience = this.characterManager.Character.Entity.GetComponent<ExperienceComponent>();

            for (var i = 0; i < levels; i++)
            {
                experience.Experience.Add(experience.Experience.RequiredNextLevel() - experience.Experience.Current);
            }

            return $"{this.characterManager.Character.Name} received {0} levels";
        }
    }
}