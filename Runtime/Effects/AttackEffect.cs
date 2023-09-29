using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class AttackEffect : Effect
    {
        private readonly AttackEffectData m_Data;

        public AttackEffect(AttackEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new AttackEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            if (!target.IsAlive() || caster.IsUncontrollable())
            {
                TriggerFinished();
                return;
            }

            var attack = m_Data.IsOffHand
                ? caster.GetComponent<SpellbookComponent>().LastWeaponSkill()
                : caster.GetComponent<SpellbookComponent>().FirstWeaponSkill();

            if (attack == null || attack.IsOnCooldown() || !attack.IsTargetInRange(target))
            {
                TriggerFinished();
                return;
            }

            if (m_Data.TriggerCooldown)
            {
                attack.RunCooldown();
            }

            Timer.Instance.Wait(0.25f, () =>
            {
                attack.FaceTargetAndPlayAnimation(target, () =>
                {
                    var effect = attack.Effect.Clone();
                    effect.Skill = attack;
                    effect.DamageMultiplier = m_Data.DamageMultiplier;
                    effect.Finished += OnEffectFinished;
                    effect.Apply(Caster, target);

                    if (effect is LaunchMissileEffect)
                    {
                        var multishots = Caster.GetComponent<BehavioursComponent>().Behaviours.Where(x => x is MultishotBehaviour).Cast<MultishotBehaviour>();

                        foreach (var multishot in multishots)
                        {
                            multishot.Multishot(Caster, target, attack, 0, m_Data.DamageMultiplier);
                        }
                    }
                });
            });
        }

        private void OnEffectFinished(Effect effect)
        {
            effect.Finished -= OnEffectFinished;

            TriggerFinished();
        }
    }
}