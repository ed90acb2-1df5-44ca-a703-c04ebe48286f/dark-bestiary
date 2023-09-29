using System;
using DarkBestiary.Components;

namespace DarkBestiary.Console
{
    public class AddExperienceConsoleCommand : IConsoleCommand
    {
        public string GetSignature()
        {
            return "add_exp";
        }

        public string GetDescription()
        {
            return "Add some experience. (Format: [amount]";
        }

        public string Execute(string input)
        {
            var strings = input.Split();
            var amount = strings.Length > 0 ? Math.Max(int.Parse(strings[0]), 1) : 1;
            var experience = Game.Instance.Character.Entity.GetComponent<ExperienceComponent>();
            var reliquary = Game.Instance.Character.Entity.GetComponent<ReliquaryComponent>();

            experience.Experience.Add(amount);
            reliquary.GiveExperience(amount);

            return $"{Game.Instance.Character.Name} received {amount} experience";
        }
    }
}