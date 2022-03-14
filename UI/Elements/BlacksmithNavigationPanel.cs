using DarkBestiary.GameStates;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class BlacksmithNavigationPanel : MonoBehaviour
    {
        [SerializeField] private Tab tabPrefab;
        [SerializeField] private Transform tabContainer;

        private TownGameState townGameState;
        private Tab craft;
        private Tab removeGems;
        private Tab forging;
        private Tab socketing;
        private Tab dismantling;

        private void Start()
        {
            if (Game.Instance.IsVisions)
            {
                gameObject.SetActive(false);
                return;
            }

            this.craft = Instantiate(this.tabPrefab, this.tabContainer);
            this.craft.Clicked += OnCraftTabClicked;
            this.craft.Construct(I18N.Instance.Get("ui_craft"));

            this.removeGems = Instantiate(this.tabPrefab, this.tabContainer);
            this.removeGems.Clicked += OnRemoveGemsClicked;
            this.removeGems.Construct(I18N.Instance.Get("ui_remove_gems"));

            this.forging = Instantiate(this.tabPrefab, this.tabContainer);
            this.forging.Clicked += OnForgingClicked;
            this.forging.Construct(I18N.Instance.Get("ui_forging"));

            this.socketing = Instantiate(this.tabPrefab, this.tabContainer);
            this.socketing.Clicked += OnSocketingClicked;
            this.socketing.Construct(I18N.Instance.Get("ui_socketing"));

            this.dismantling = Instantiate(this.tabPrefab, this.tabContainer);
            this.dismantling.Clicked += OnDismantlingClicked;
            this.dismantling.Construct(I18N.Instance.Get("ui_dismantling"));

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
            this.craft.SetSelected(this.townGameState.IsCraft);
            this.removeGems.SetSelected(this.townGameState.IsRemoveGems);
            this.forging.SetSelected(this.townGameState.IsForging);
            this.socketing.SetSelected(this.townGameState.IsSocketing);
            this.dismantling.SetSelected(this.townGameState.IsDismantling);
        }

        private void OnDismantlingClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.DismantlingController.View);
        }

        private void OnSocketingClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.SocketingController.View);
        }

        private void OnForgingClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.ItemForgingViewController.View);
        }

        private void OnRemoveGemsClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.RemoveGemsController.View);
        }

        private void OnCraftTabClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.BlacksmithController.View);
        }
    }
}