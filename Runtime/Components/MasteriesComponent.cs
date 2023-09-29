using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Masteries;

namespace DarkBestiary.Components
{
    public class MasteriesComponent : Component
    {

        public IReadOnlyCollection<Mastery> Masteries { get; private set; }

        public MasteriesComponent Construct(IReadOnlyCollection<Mastery> masteries)
        {
            Masteries = masteries;
            return this;
        }

        protected override void OnInitialize()
        {
            foreach (var mastery in Masteries)
            {
                mastery.Initialize(gameObject);
            }
        }

        protected override void OnTerminate()
        {
            foreach (var mastery in Masteries)
            {
                mastery.Terminate();
            }
        }

        public Mastery Find(int masteryId)
        {
            return Masteries.First(m => m.Id == masteryId);
        }
    }
}