using DarkBestiary.Messaging;

namespace DarkBestiary.Skills
{
    public class SkillSlot
    {
        public event Payload<SkillSlot> SkillChanged;

        public int Index { get; }
        public Skill Skill { get; private set; }
        public SkillType SkillType { get; }

        public SkillSlot(int index, SkillType skillType)
        {
            Index = index;
            SkillType = skillType;
            Skill = Skill.Empty;
        }

        public void ChangeSkill(Skill skill)
        {
            Skill = skill;
            SkillChanged?.Invoke(this);
        }
    }
}