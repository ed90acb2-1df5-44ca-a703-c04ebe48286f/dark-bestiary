using System;

namespace DarkBestiary.Skills
{
    public class SkillSlot
    {
        public event Action<SkillSlot> SkillChanged;

        public int Index { get; }
        public Skill Skill { get; private set; }
        public SkillType SkillType { get; }
        public bool IsEmpty => Skill.IsEmpty();

        public SkillSlot(int index, SkillType skillType)
        {
            Index = index;
            SkillType = skillType;
            Skill = Skill.s_Empty;
        }

        public void ChangeSkill(Skill skill)
        {
            Skill = skill;
            SkillChanged?.Invoke(this);
        }
    }
}