using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Managers;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class AddCurrencyEffect : Effect
    {
        private readonly AddCurrencyEffectData m_Data;

        public AddCurrencyEffect(AddCurrencyEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new AddCurrencyEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var amount = (int) Formula.Evaluate(m_Data.CurrencyFormula, caster, target);

            target.GetComponent<CurrenciesComponent>().Give(m_Data.CurrencyType, amount);

            FloatingTextManager.Instance.Enqueue(target, $"+{amount}", new Color(1, 0.85f, 0));

            TriggerFinished();
        }
    }
}