using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class RemoveSkillConsoleCommand : IConsoleCommand
    {
        private readonly ISkillRepository m_SkillRepository;

        public RemoveSkillConsoleCommand(ISkillRepository skillRepository)
        {
            m_SkillRepository = skillRepository;
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
            var skill = m_SkillRepository.FindOrFail(int.Parse(options[0]));

            Game.Instance.Character.Entity.GetComponent<SpellbookComponent>().Unlearn(skill);

            return $"Removed skill: {skill.Name}";
        }
    }
}