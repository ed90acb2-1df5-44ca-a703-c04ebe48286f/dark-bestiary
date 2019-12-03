using DarkBestiary.GameBoard;

namespace DarkBestiary.Skills.Targeting
{
    public class NoneSkillUseStrategy : ISkillUseStrategy
    {
        public I18NString Name => I18N.Instance.Get("ui_skill_target_none");

        public void Use(Skill skill, BoardCell cell)
        {
            skill.Use(skill.Caster);
        }

        public bool IsValidTarget(Skill skill, object target)
        {
            return true;
        }

        public bool IsValidCell(Skill skill, BoardCell cell)
        {
            return true;
        }
    }
}