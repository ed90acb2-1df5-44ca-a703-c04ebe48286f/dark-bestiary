using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Skills;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class MultishotBehaviour : Behaviour
    {
        private readonly BoardNavigator boardNavigator;

        public MultishotBehaviour(BehaviourData data, BoardNavigator boardNavigator,
            List<Validator> validators) : base(data, validators)
        {
            this.boardNavigator = boardNavigator;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Skill.AnySkillUsing += OnAnySkillUsing;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            Skill.AnySkillUsing -= OnAnySkillUsing;
        }

        private void OnAnySkillUsing(SkillUseEventData data)
        {
            if (data.Caster != Target || data.Skill.Type != SkillType.Weapon || !data.Skill.Flags.HasFlag(SkillFlags.RangedWeapon))
            {
                return;
            }

            if (!(data.Skill.Effect is LaunchMissileEffect launchMissileEffect))
            {
                return;
            }

            var enemies = this.boardNavigator
                .WithinCircle(data.Caster.transform.position, data.Skill.GetMaxRange())
                .Where(cell => cell.IsOccupied && cell.OccupiedBy != (GameObject) data.Target &&
                               cell.IsOnLineOfSight(data.Caster.transform.position) &&
                               !cell.OccupiedBy.IsDummy() &&
                               cell.OccupiedBy != data.Caster && cell.OccupiedBy.IsEnemyOf(data.Caster) &&
                               Vector3.Dot((data.Target.GetPosition() - data.Caster.transform.position).normalized,
                                   (cell.transform.position - data.Caster.transform.position).normalized) >= 0.5f)
                .OrderBy(cell => (cell.transform.position - data.Caster.transform.position).sqrMagnitude)
                .Take(2);

            launchMissileEffect.Skill = data.Skill;

            Timer.Instance.Wait(0.2f, () =>
            {
                foreach (var enemy in enemies)
                {
                    launchMissileEffect.Clone().Apply(data.Caster, enemy.OccupiedBy);
                }
            });
        }
    }
}