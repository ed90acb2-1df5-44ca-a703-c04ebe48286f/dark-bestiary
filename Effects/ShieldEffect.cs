using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;
using DarkBestiary.Properties;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ShieldEffect : FormulaBasedEffect
    {
        private readonly ShieldEffectData data;
        private readonly IBehaviourRepository behaviourRepository;

        public ShieldEffect(ShieldEffectData data, List<ValidatorWithPurpose> validators,
            IBehaviourRepository behaviourRepository) : base(data, validators)
        {
            this.data = data;
            this.behaviourRepository = behaviourRepository;
        }

        protected override Effect New()
        {
            return new ShieldEffect(this.data, this.Validators, this.behaviourRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var behaviour = this.behaviourRepository.Find(this.data.BehaviourId);

            if (behaviour is ShieldBehaviour shieldBehaviour)
            {
                var amount = GetAmount(caster, target);

                if (amount > 0)
                {
                    shieldBehaviour.Caster = caster;
                    shieldBehaviour.Target = target;
                    shieldBehaviour.ChangeAmount(amount);

                    target.GetComponent<BehavioursComponent>().ApplyAllStacks(shieldBehaviour, caster);
                }
            }

            TriggerFinished();
        }

        private float GetAmount(GameObject caster, GameObject target)
        {
            var amount = EvaluateFormula(caster, target) * StackCount;

            amount *= 1 + caster.GetComponent<PropertiesComponent>().Get(PropertyType.HealingIncrease).Value();

            return amount;
        }

        public string GetAmountString(GameObject entity)
        {
            return Wrap($"{(int) GetAmount(entity, null)}");
        }

        private string Wrap(string value)
        {
            var wrapped = $"<color=#E6CC80>{value}</color>";

            if (SettingsManager.Instance.DisplayFormulasInTooltips)
            {
                wrapped += $" <color=#888888>({this.data.Formula})</color>";
            }

            return wrapped;
        }
    }
}