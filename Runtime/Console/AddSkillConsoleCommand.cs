using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class AddSkillConsoleCommand : IConsoleCommand
    {
        private readonly ISkillRepository m_SkillRepository;

        public AddSkillConsoleCommand(ISkillRepository skillRepository)
        {
            m_SkillRepository = skillRepository;
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
            var skill = m_SkillRepository.FindOrFail(int.Parse(options[0]));

            Game.Instance.Character.Entity.GetComponent<SpellbookComponent>().Learn(skill);

            return $"Added skill: {skill.Name}";
        }
    }
}