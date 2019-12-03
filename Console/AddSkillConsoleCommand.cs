using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddSkillConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly ISkillRepository skillRepository;

        public AddSkillConsoleCommand(CharacterManager characterManager, ISkillRepository skillRepository)
        {
            this.characterManager = characterManager;
            this.skillRepository = skillRepository;
        }

        public string GetSignature()
        {
            return "add_skill";
        }

        public string GetDescription()
        {
            return "Unlocks skill by id (Format: [skillId])";
        }

        public string Execute(string input)
        {
            var options = input.Split();
            var skill = this.skillRepository.Find(int.Parse(options[0]));

            this.characterManager.Character.Entity.GetComponent<SpellbookComponent>().Add(skill, false);

            return $"Added skill: {skill.Name}";
        }
    }
}