using System;
using DarkBestiary.Components;

namespace DarkBestiary.Console
{
    public class LevelupConsoleCommand : IConsoleCommand
    {
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
            var experience = Game.Instance.Character.Entity.GetComponent<ExperienceComponent>();

            for (var i = 0; i < levels; i++)
            {
                experience.Experience.Add(experience.Experience.RequiredNextLevel() - experience.Experience.Current);
            }

            return $"{Game.Instance.Character.Name} received {0} levels";
        }
    }
}