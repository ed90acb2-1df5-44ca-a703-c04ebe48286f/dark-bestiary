using System;
using DarkBestiary.Components;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddExperienceConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;

        public AddExperienceConsoleCommand(CharacterManager characterManager)
        {
            this.characterManager = characterManager;
        }

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
            var experience = this.characterManager.Character.Entity.GetComponent<ExperienceComponent>();
            var reliquary = this.characterManager.Character.Entity.GetComponent<ReliquaryComponent>();

            experience.Experience.Add(amount);
            reliquary.GiveExperience(amount);

            return $"{this.characterManager.Character.Name} received {amount} experience";
        }
    }
}