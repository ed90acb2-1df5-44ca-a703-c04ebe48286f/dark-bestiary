using DarkBestiary.Components;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddSkillPointsConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;

        public AddSkillPointsConsoleCommand(CharacterManager characterManager)
        {
            this.characterManager = characterManager;
        }

        public string GetSignature()
        {
            return "add_skill_points";
        }

        public string GetDescription()
        {
            return "Give character skill points. (Format: [amount]";
        }

        public string Execute(string input)
        {
            var points = int.Parse(input.Split()[0]);
            this.characterManager.Character.Entity.GetComponent<SpecializationsComponent>().SkillPoints += points;

            return $"{this.characterManager.Character.Name} received {points} skill points.";
        }
    }
}