using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Managers;
using DarkBestiary.Properties;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class HealEffect : FormulaBasedEffect
    {
        private readonly HealEffectData m_Data;

        public HealEffect(HealEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new HealEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var amount = GetAmount(caster, target);

            if (target.GetComponent<BehavioursComponent>().IsUndead && !m_Data.Flags.HasFlag(HealFlags.HealUndead))
            {
                target.GetComponent<HealthComponent>().Damage(caster, new Damage(amount, DamageType.Health, WeaponSound.None, DamageFlags.None, DamageInfoFlags.None, Skill));
            }
            else
            {
                target.GetComponent<HealthComponent>().Heal(caster, new Healing(amount, m_Data.Flags, Skill));
            }

            TriggerFinished();
        }

        private float GetAmount(GameObject caster, GameObject target)
        {
            var amount = m_Data.Base + EvaluateFormula(caster, target) * StackCount;

            amount *= 1 + caster.GetComponent<PropertiesComponent>().Get(PropertyType.HealingIncrease).Value();

            return amount;
        }

        public string GetAmountString(GameObject entity)
        {
            return Wrap($"{(int) GetAmount(entity, null)}");
        }

        private string Wrap(string value)
        {
            var wrapped = $"<color=#1EFF00>{value}</color>";

            if (SettingsManager.Instance.DisplayFormulasInTooltips)
            {
                wrapped += $" <color=#888888>({m_Data.Formula})</color>";
            }

            return wrapped;
        }
    }
}