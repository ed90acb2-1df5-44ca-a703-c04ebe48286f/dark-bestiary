using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class BlacksmithNavigationPanel : MonoBehaviour
    {
        [SerializeField] private Tab tabPrefab;
        [SerializeField] private Transform tabContainer;

        private Tab craft;
        private Tab removeGems;
        private Tab forging;
        private Tab socketing;
        private Tab dismantling;

        private void Start()
        {
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

            TownController.Active.ViewSwitched += OnViewSwitched;

            UpdateSelectedTab();
        }

        private void OnDestroy()
        {
            TownController.Active.ViewSwitched -= OnViewSwitched;
        }

        private void OnViewSwitched(IView view)
        {
            UpdateSelectedTab();
        }

        private void UpdateSelectedTab()
        {
            var town = TownController.Active;

            this.craft.SetSelected(town.IsCraft);
            this.removeGems.SetSelected(town.IsRemoveGems);
            this.forging.SetSelected(town.IsForging);
            this.socketing.SetSelected(town.IsSocketing);
            this.dismantling.SetSelected(town.IsDismantling);
        }

        private void OnDismantlingClicked(Tab tab)
        {
            TownController.Active.ShowDismantling();
        }

        private void OnSocketingClicked(Tab tab)
        {
            TownController.Active.ShowSocketing();
        }

        private void OnForgingClicked(Tab tab)
        {
            TownController.Active.ShowForging();
        }

        private void OnRemoveGemsClicked(Tab tab)
        {
            TownController.Active.ShowRemoveGems();
        }

        private void OnCraftTabClicked(Tab tab)
        {
            TownController.Active.ShowCraft();
        }
    }
}