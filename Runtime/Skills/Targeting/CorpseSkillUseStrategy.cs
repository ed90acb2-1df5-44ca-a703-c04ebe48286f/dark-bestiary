using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Skills.Targeting
{
    public class CorpseSkillUseStrategy : ISkillUseStrategy
    {
        public I18NString Name => I18N.Instance.Get("ui_skill_target_corpse");

        public void Use(Skill skill, BoardCell cell)
        {
            var corpse = cell.GameObjectsInside.Corpses().First();

            skill.Use(cell.transform.position);

            corpse.Consume();
        }

        public bool IsValidTarget(Skill skill, object target)
        {
            return target is Vector3;
        }

        public bool IsValidCell(Skill skill, BoardCell cell)
        {
            return !cell.IsOccupied && cell.GameObjectsInside.Corpses().Any();
        }
    }
}