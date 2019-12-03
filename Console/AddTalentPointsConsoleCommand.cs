using DarkBestiary.Components;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddTalentPointsConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;

        public AddTalentPointsConsoleCommand(CharacterManager characterManager)
        {
            this.characterManager = characterManager;
        }

        public string GetSignature()
        {
            return "add_talent_points";
        }

        public string GetDescription()
        {
            return "Give character talent points. (Format: [amount]";
        }

        public string Execute(string input)
        {
            var points = int.Parse(input.Split()[0]);
            this.characterManager.Character.Entity.GetComponent<TalentsComponent>().Points += points;

            return $"{this.characterManager.Character.Name} received {points} talent points.";
        }
    }
}