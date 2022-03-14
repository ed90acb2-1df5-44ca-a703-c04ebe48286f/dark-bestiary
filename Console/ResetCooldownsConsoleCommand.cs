using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.Visions;

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

            if (VisionManager.Instance != null)
            {
                foreach (var skillSlot in VisionManager.Instance.SkillSlots)
                {
                    skillSlot.Skill.ResetCooldown();
                }
            }

            return "Cooldowns are reset.";
        }
    }
}