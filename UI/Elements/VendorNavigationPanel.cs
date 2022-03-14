using DarkBestiary.GameStates;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class VendorNavigationPanel : MonoBehaviour
    {
        [SerializeField] private Tab tabPrefab;
        [SerializeField] private Transform tabContainer;

        private TownGameState townGameState;
        private Tab vendor;
        private Tab gamble;

        private void Start()
        {
            if (Game.Instance.IsVisions)
            {
                gameObject.SetActive(false);
                return;
            }

            this.vendor = Instantiate(this.tabPrefab, this.tabContainer);
            this.vendor.Clicked += OnVendorClicked;
            this.vendor.Construct(I18N.Instance.Get("ui_trade"));

            this.gamble = Instantiate(this.tabPrefab, this.tabContainer);
            this.gamble.Clicked += OnGambleClicked;
            this.gamble.Construct(I18N.Instance.Get("ui_black_market"));

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
            this.vendor.SetSelected(this.townGameState.IsVendor);
            this.gamble.SetSelected(this.townGameState.IsGamble);
        }

        private void OnVendorClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.VendorController.View);
        }

        private void OnGambleClicked(Tab tab)
        {
            this.townGameState.SwitchView(this.townGameState.GambleController.View);
        }
    }
}