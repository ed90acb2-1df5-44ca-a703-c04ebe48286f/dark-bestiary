using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Skills.Targeting
{
    public class EnemyUnitSkillUseStrategy : ISkillUseStrategy
    {
        public I18NString Name => I18N.Instance.Get("ui_skill_target_enemy_unit");

        public void Use(Skill skill, BoardCell cell)
        {
            skill.Use(cell.OccupiedBy);
        }

        public bool IsValidTarget(Skill skill, object target)
        {
            var targetGameObject = target as GameObject;

            return targetGameObject != null &&
                   targetGameObject.IsAlive() &&
                   targetGameObject.IsEnemyOf(skill.Caster) &&
                   !targetGameObject.IsDummy();
        }

        public bool IsValidCell(Skill skill, BoardCell cell)
        {
            return cell.IsOccupied &&
                   cell.OccupiedBy.IsAlive() &&
                   cell.OccupiedBy.IsEnemyOf(skill.Caster) &&
                   !cell.OccupiedBy.IsDummy();
        }
    }
}