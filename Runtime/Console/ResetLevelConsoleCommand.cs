using System;
using DarkBestiary.Components;

namespace DarkBestiary.Console
{
    public class ResetLevelConsoleCommand : IConsoleCommand
    {
        public string GetSignature()
        {
            return "reset_level";
        }

        public string GetDescription()
        {
            return "Set character level. (Format: [level]";
        }

        public string Execute(string input)
        {
            var strings = input.Split();
            var level = strings.Length > 0 ? Math.Max(int.Parse(strings[0]), 1) : 1;
            var character = Game.Instance.Character;

            character.Entity.GetComponent<ExperienceComponent>().Experience.ResetToLevel(level);

            return $"{character.Name}'s level set to {level}.";
        }
    }
}