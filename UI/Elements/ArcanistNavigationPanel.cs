using DarkBestiary.GameStates;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ArcanistNavigationPanel : MonoBehaviour
    {
        [SerializeField] private Tab tabPrefab;
        [SerializeField] private Transform tabContainer;

        private TownGameState townGameState;
        private Tab bestiary;
        private Tab alchemy;
        private Tab transmutation;
        private Tab sphereCraft;

        private void Start()
        {
            this.bestiary = Instantiate(this.tabPrefab, this.tabContainer);
            this.bestiary.Clicked += OnBestiaryClicked;
            this.bestiary.Construct(I18N.Instance.Get("ui_bestiary"));

            this.alchemy = Instantiate(this.tabPrefab, this.tabContainer);
            this.alchemy.Clicked += OnAlchemyClicked;
            this.alchemy.Construct(I18N.Instance.Get("ui_alchemy"));

            this.transmutation = Instantiate(this.tabPrefab, this.tabContainer);
            this.transmutation.Clicked += OnTransmutationClicked;
            this.transmutation.Construct(I18N.Instance.Get("ui_transmutation"));

            this.sphereCraft = Instantiate(this.tabPrefab, this.tabContainer);
            this.sphereCraft.Clicked += OnSphereCraftClicked;
            this.sphereCraft.Construct(I18N.Instance.Get("ui_sphere_craft"));

            this.townGameState = Game.Instance.State as TownGameState;

            TownGameState.Entered += OnEnteredTown;

            UpdateSelectedTab();
        }

        private void OnEnable()
        {
            if (this.townGameState == null)
            {
                return;
            }

            Timer.Instance.WaitForFixedUpdate(UpdateSelectedTab);
        }

        private void OnDestroy()
        {
            TownGameState.Entered -= OnEnteredTown;

            if (this.townGameState != null)
            {
                this.townGameState.ViewSwitched -= OnViewSwitched;
            }
        }

        private void OnEnteredTown(TownGameState townGameState)
        {
            if (this.townGameState != null)
            {
                this.townGameState.ViewSwitched -= OnViewSwitched;
            }

            this.townGameState = townGameState;
            this.townGameState.ViewSwitched += OnViewSwitched;

            UpdateSelectedTab();
        }

        private void OnViewSwitched(IView view)
        {
            UpdateSelectedTab();
        }

        private void UpdateSelectedTab()
        {
            this.bestiary.SetSelected(this.townGameState.IsBestiary);
            this.alchemy.SetSelected(this.townGameState.IsAlchemy);
            this.transmutation.SetSelected(this.townGameState.IsTransmutation);
            this.sphereCraft.SetSelected(this.townGameState.IsSphereCraft);
        }

        private void OnBestiaryClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.BestiaryController.View);
        }

        private void OnAlchemyClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.AlchemyController.View);
        }

        private void OnTransmutationClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.TransmutationController.View);
        }

        private void OnSphereCraftClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.SphereCraftController.View);
        }
    }
}