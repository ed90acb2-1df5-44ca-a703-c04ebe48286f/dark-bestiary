using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class RemoveSkillConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly ISkillRepository skillRepository;

        public RemoveSkillConsoleCommand(CharacterManager characterManager, ISkillRepository skillRepository)
        {
            this.characterManager = characterManager;
            this.skillRepository = skillRepository;
        }

        public string GetSignature()
        {
            return "remove_skill";
        }

        public string GetDescription()
        {
            return "Removes skill by id (Format: [skillId])";
        }

        public string Execute(string input)
        {
            var options = input.Split();
            var skill = this.skillRepository.Find(int.Parse(options[0]));

            this.characterManager.Character.Entity.GetComponent<SpellbookComponent>().Remove(skill);

            return $"Removed skill: {skill.Name}";
        }
    }
}