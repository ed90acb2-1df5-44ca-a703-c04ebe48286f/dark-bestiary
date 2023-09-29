using DarkBestiary.GameBoard;

namespace DarkBestiary.Skills.Targeting
{
    public interface ISkillUseStrategy
    {
        I18NString Name { get; }

        void Use(Skill skill, BoardCell cell);

        bool IsValidTarget(Skill skill, object target);

        bool IsValidCell(Skill skill, BoardCell cell);
    }
}