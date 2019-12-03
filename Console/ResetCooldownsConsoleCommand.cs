using DarkBestiary.Components;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class ResetCooldownsConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;

        public ResetCooldownsConsoleCommand(CharacterManager characterManager)
        {
            this.characterManager = characterManager;
        }

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
            this.characterManager.Character.Entity.GetComponent<SpellbookComponent>().ResetCooldowns();
            this.characterManager.Character.Entity.GetComponent<ResourcesComponent>().Restore();

            return "Cooldowns are reset.";
        }
    }
}