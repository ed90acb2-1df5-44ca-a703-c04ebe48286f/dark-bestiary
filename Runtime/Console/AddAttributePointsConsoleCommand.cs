using DarkBestiary.Components;

namespace DarkBestiary.Console
{
    public class AddAttributePointsConsoleCommand : IConsoleCommand
    {
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
            Game.Instance.Character.Entity.GetComponent<AttributesComponent>().Points += points;

            return $"{Game.Instance.Character.Name} received {points} attribute points.";
        }
    }
}