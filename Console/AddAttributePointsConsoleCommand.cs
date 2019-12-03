using DarkBestiary.Components;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddAttributePointsConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;

        public AddAttributePointsConsoleCommand(CharacterManager characterManager)
        {
            this.characterManager = characterManager;
        }

        public string GetSignature()
        {
            return "add_attribute_points";
        }

        public string GetDescription()
        {
            return "Give character attribute points. (Format: [amount]";
        }

        public string Execute(string input)
        {
            var points = int.Parse(input.Split()[0]);
            this.characterManager.Character.Entity.GetComponent<AttributesComponent>().Points += points;

            return $"{this.characterManager.Character.Name} received {points} attribute points.";
        }
    }
}