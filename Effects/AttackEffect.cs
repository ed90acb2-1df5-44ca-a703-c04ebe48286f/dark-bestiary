using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Skills;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class AttackEffect : Effect
    {
        private static bool isAttacking;

        private readonly EffectData data;

        public AttackEffect(EffectData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new AttackEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            if (!target.IsAlive())
            {
                TriggerFinished();
                return;
            }

            var attack = caster.GetComponent<SpellbookComponent>().Slots
                .Where(s => s.Skill.Type == SkillType.Weapon)
                .OrderBy(s => s.Index)
                .FirstOrDefault()?
                .Skill;

            if (attack == null || attack.IsOnCooldown() || !attack.IsTargetInRange(target))
            {
                TriggerFinished();
                return;
            }

            Timer.Instance.Wait(0.25f, () =>
            {
                attack.FaceTargetAndPlayAnimation(target, () =>
                {
                    isAttacking = true;

                    var effect = attack.Effect.Clone();
                    effect.Skill = attack;
                    effect.Finished += OnEffectFinished;
                    effect.Apply(Caster, target);
                });
            });
        }

        private void OnEffectFinished(Effect effect)
        {
            isAttacking = false;

            effect.Finished -= OnEffectFinished;

            TriggerFinished();
        }
    }
}