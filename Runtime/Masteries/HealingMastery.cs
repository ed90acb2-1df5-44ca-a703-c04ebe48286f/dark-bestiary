using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Events;
using DarkBestiary.Items;

namespace DarkBestiary.Masteries
{
    public class HealingMastery : Mastery
    {
        public HealingMastery(MasteryData data, ItemModifier modifier) : base(data, modifier)
        {
        }

        protected override void OnInitialize()
        {
            HealthComponent.AnyEntityHealed += OnAnyEntityHealed;
            BehavioursComponent.AnyBehaviourApplied += OnAnyBehaviourApplied;
        }

        protected override void OnTerminate()
        {
            HealthComponent.AnyEntityHealed -= OnAnyEntityHealed;
            BehavioursComponent.AnyBehaviourApplied -= OnAnyBehaviourApplied;
        }

        private void OnAnyBehaviourApplied(Behaviour behaviour)
        {
            if (!(behaviour is ShieldBehaviour) || behaviour.Caster != Owner)
            {
                return;
            }

            Experience.Add(4);
        }

        private void OnAnyEntityHealed(EntityHealedEventData data)
        {
            if (data.Source != Owner || data.Healing.IsVampirism() || data.Healing.IsRegeneration())
            {
                return;
            }

            Experience.Add(4);
        }
    }
}