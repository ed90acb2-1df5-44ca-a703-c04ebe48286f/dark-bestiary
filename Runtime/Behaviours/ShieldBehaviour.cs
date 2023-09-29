using System;
using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Managers;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class ShieldBehaviour : Behaviour
    {
        public static Action<ShieldBehaviour> AnyShieldChanged;

        public float Amount { get; private set; }

        private readonly ShieldBehaviourData m_Data;

        public ShieldBehaviour(ShieldBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        public string GetMaxAmountString(GameObject entity)
        {
            if (string.IsNullOrEmpty(m_Data.MaxAmountFormula))
            {
                return "0";
            }

            return Wrap(((int) Formula.Evaluate(m_Data.MaxAmountFormula, entity, Target)).ToString());
        }

        private string Wrap(string value)
        {
            return SettingsManager.Instance.DisplayFormulasInTooltips ?
                $"{value} <color=#888888>({m_Data.MaxAmountFormula})</color>" : value;
        }

        public void ChangeAmount(float amount)
        {
            Amount = amount;

            if (!string.IsNullOrEmpty(m_Data.MaxAmountFormula))
            {
                Amount = Mathf.Clamp(Amount, 0, Formula.Evaluate(m_Data.MaxAmountFormula, Caster, Target));
            }

            AnyShieldChanged?.Invoke(this);

            if (Math.Abs(Amount) < 1)
            {
                Remove();
            }
        }

        public float Absorb(ref Damage damage)
        {
            float absorbed;

            if (Amount >= damage)
            {
                absorbed = damage;
                ChangeAmount(Amount - damage);
                damage.Amount *= 0;
            }
            else
            {
                absorbed = Amount;
                damage.Amount -= Amount;
                ChangeAmount(0);
            }

            return absorbed;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            AnyShieldChanged?.Invoke(this);
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            AnyShieldChanged?.Invoke(this);
        }

        protected override void OnStackEffect(Behaviour behaviour)
        {
            ChangeAmount(Amount + ((ShieldBehaviour) behaviour).Amount);
        }

        protected override void OnRefreshEffect(Behaviour behaviour)
        {
            ChangeAmount(((ShieldBehaviour) behaviour).Amount);
        }
    }
}