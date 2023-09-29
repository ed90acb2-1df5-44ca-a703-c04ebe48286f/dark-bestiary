using DarkBestiary.Components;

namespace DarkBestiary.Console
{
    public class AddTalentPointsConsoleCommand : IConsoleCommand
    {
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
            Game.Instance.Character.Entity.GetComponent<TalentsComponent>().Points += points;

            return $"{Game.Instance.Character.Name} received {points} talent points.";
        }
    }
}