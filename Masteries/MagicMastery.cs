using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.Masteries
{
    public class MagicMastery : Mastery
    {
        public MagicMastery(MasteryData data, ItemModifier modifier) : base(data, modifier)
        {
        }

        protected override void OnInitialize()
        {
            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
        }

        protected override void OnTerminate()
        {
            HealthComponent.AnyEntityDamaged -= OnAnyEntityDamaged;
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Attacker != Owner || !data.Damage.IsMagic())
            {
                return;
            }

            if (this.Data.DamageType != data.Damage.Type)
            {
                return;
            }

            Experience.Add(1);
        }
    }
}