using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Data.Repositories;
using DarkBestiary.UI.Views;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Controllers
{
    public class BuffSelectionViewController : ViewController<IBuffSelectionView>
    {
        public Behaviour SelectedBuff { get; private set; }

        private readonly List<Behaviour> m_Behaviours;

        public BuffSelectionViewController(IBuffSelectionView view,
            IBehaviourRepository behaviourRepository, List<Behaviour> behaviours = null) : base(view)
        {
            m_Behaviours = behaviours == null || behaviours.Count == 0
                ? behaviourRepository.Random(b => b.Flags.HasFlag(BehaviourFlags.Ascension), 4) : behaviours;
        }

        protected override void OnInitialize()
        {
            View.BuffSelected += OnBuffSelected;
            View.Hidden += ViewOnHidden;
            View.Construct(m_Behaviours);
        }

        protected override void OnTerminate()
        {
            View.BuffSelected -= OnBuffSelected;
            View.Hidden -= ViewOnHidden;
        }

        private void ViewOnHidden()
        {
            Terminate();
        }

        private void OnBuffSelected(Behaviour behaviour)
        {
            SelectedBuff = behaviour;
        }
    }
}