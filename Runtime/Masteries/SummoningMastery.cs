using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Items;

namespace DarkBestiary.Masteries
{
    public class SummoningMastery : Mastery
    {
        public SummoningMastery(MasteryData data, ItemModifier modifier) : base(data, modifier)
        {
        }

        protected override void OnInitialize()
        {
            Component.AnyComponentInitialized += OnAnyComponentInitialized;
        }

        protected override void OnTerminate()
        {
            Component.AnyComponentInitialized -= OnAnyComponentInitialized;
        }

        private void OnAnyComponentInitialized(Component component)
        {
            if (!(component is SummonedComponent summoned) || component.gameObject.IsDummy())
            {
                return;
            }

            if (summoned.Master != Owner)
            {
                return;
            }

            Experience.Add(8);
        }
    }
}