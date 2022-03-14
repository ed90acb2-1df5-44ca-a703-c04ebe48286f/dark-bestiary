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
            List<ValidatorWithPurpose> validators) : base(data, validators)
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

            Multishot(data.Caster, data.Target, data.Skill, 0.2f);
        }

        public void Multishot(GameObject caster, object target, Skill skill, float timeout, float damageMultiplier = 1)
        {
            if (!(skill.Effect is LaunchMissileEffect launchMissileEffect))
            {
                return;
            }

            var enemies = this.boardNavigator
                .WithinCircle(caster.transform.position, skill.GetMaxRange())
                .Where(cell => cell.IsOccupied && cell.OccupiedBy != (GameObject) target &&
                               cell.IsLineOfSightWalkable(caster.transform.position) &&
                               !cell.OccupiedBy.IsDummy() &&
                               cell.OccupiedBy != caster && cell.OccupiedBy.IsEnemyOf(caster) &&
                               Vector3.Dot((target.GetPosition() - caster.transform.position).normalized,
                                   (cell.transform.position - caster.transform.position).normalized) >= 0.5f)
                .OrderBy(cell => (cell.transform.position - caster.transform.position).sqrMagnitude)
                .Take(2);

            Timer.Instance.Wait(timeout, () =>
            {
                foreach (var enemy in enemies)
                {
                    var effect = launchMissileEffect.Clone();
                    effect.Skill = skill;
                    effect.DamageMultiplier = damageMultiplier;
                    effect.Apply(caster, enemy.OccupiedBy);
                }
            });
        }
    }
}