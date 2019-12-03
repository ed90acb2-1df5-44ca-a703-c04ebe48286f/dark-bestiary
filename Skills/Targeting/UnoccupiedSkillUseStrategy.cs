using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Skills.Targeting
{
    public class UnoccupiedSkillUseStrategy : ISkillUseStrategy
    {
        public I18NString Name => I18N.Instance.Get("ui_skill_target_point");

        public void Use(Skill skill, BoardCell cell)
        {
            skill.Use(cell.transform.position);
        }

        public bool IsValidTarget(Skill skill, object target)
        {
            return target is Vector3;
        }

        public bool IsValidCell(Skill skill, BoardCell cell)
        {
            return !cell.IsOccupied;
        }
    }
}