using DarkBestiary.Components;

namespace DarkBestiary.Console
{
    public class ResetCooldownsConsoleCommand : IConsoleCommand
    {
        public string GetSignature()
        {
            return "reset";
        }

        public string GetDescription()
        {
            return "Reset all skill cooldowns and restore resources.";
        }

        public string Execute(string input)
        {
            Game.Instance.Character.Entity.GetComponent<SpellbookComponent>().ResetCooldowns();
            Game.Instance.Character.Entity.GetComponent<ResourcesComponent>().Restore();

            return "Cooldowns are reset.";
        }
    }
}