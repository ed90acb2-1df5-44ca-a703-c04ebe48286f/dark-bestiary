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
        private readonly AddCurrencyEffectData data;

        public AddCurrencyEffect(AddCurrencyEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new AddCurrencyEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var amount = (int) Formula.Evaluate(this.data.CurrencyFormula, caster, target);

            target.GetComponent<CurrenciesComponent>().Give(this.data.CurrencyType, amount);

            FloatingTextManager.Instance.Enqueue(target, $"+{amount}", new Color(1, 0.85f, 0));

            TriggerFinished();
        }
    }
}