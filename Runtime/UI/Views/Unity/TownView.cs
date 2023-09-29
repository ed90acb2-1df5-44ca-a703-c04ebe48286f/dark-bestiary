using System;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TownView : View, ITownView
    {
        public event Action? VendorRequested;
        public event Action? SkillVendorRequested;
        public event Action? AlchemyRequested;
        public event Action? BlacksmithCraftRequested;
        public event Action? ForgingRequested;
        public event Action? DismantlingRequested;
        public event Action? BestiaryRequested;
        public event Action? RemoveGemsRequested;
        public event Action? SocketingRequested;
        public event Action? MapRequested;
        public event Action? StashRequested;
        public event Action? GambleRequested;
        public event Action? TransmutationRequested;
        public event Action? SphereCraftRequested;
        public event Action? EateryRequested;
        public event Action? ForgottenDepthsRequested;
        public event Action? RuneInscriptionRequested;
        public event Action? RuneCraftRequested;
        public event Action? LeaderboardRequested;

        [SerializeField] private Building m_Vendor = null!;
        [SerializeField] private Building m_Arcanist = null!;
        [SerializeField] private Building m_Blacksmith = null!;
        [SerializeField] private Building m_Tavern = null!;
        [SerializeField] private Building m_Map = null!;

        protected override void OnInitialize()
        {
            m_Vendor.Construct(I18N.Instance.Get("ui_vendor"));
            m_Vendor.MouseUp += OnVendorMouseUp;

            m_Arcanist.Construct(I18N.Instance.Get("ui_arcanist"));
            m_Arcanist.MouseUp += OnArcanistMouseUp;

            m_Blacksmith.Construct(I18N.Instance.Get("ui_craftsman"));
            m_Blacksmith.MouseUp += OnCraftsmanMouseUp;

            m_Tavern.Construct(I18N.Instance.Get("ui_tavern"));
            m_Tavern.MouseUp += OnTavernMouseUp;

            m_Map.Construct(I18N.Instance.Get("ui_map"));
            m_Map.MouseUp += OnCommandBoardMouseUp;
        }

        private void OnVendorMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_vendor"))
                .AddSpace();

            dialog
                .AddOption(I18N.Instance.Get("ui_black_market"), () => GambleRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_trade"), () => VendorRequested?.Invoke())
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(m_Vendor.transform.position));
        }

        private void OnArcanistMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_arcanist"))
                .AddSpace();

            dialog
                .AddOption(I18N.Instance.Get("ui_bestiary"), () => BestiaryRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_skills"), () => SkillVendorRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_alchemy"), () => AlchemyRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_transmutation"), () => TransmutationRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_sphere_craft"), () => SphereCraftRequested?.Invoke());

            dialog.AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(m_Arcanist.transform.position));
        }

        private void OnCraftsmanMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_craftsman"))
                .AddSpace();

            dialog
                .AddOption(I18N.Instance.Get("ui_craft"), () => BlacksmithCraftRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_remove_gems"), () => RemoveGemsRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_forging"), () => ForgingRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_socketing"), () => SocketingRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_dismantling"), () => DismantlingRequested?.Invoke())
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(m_Blacksmith.transform.position));
        }

        private void OnTavernMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_tavern"))
                .AddSpace();

            dialog
                .AddOption(I18N.Instance.Get("ui_stash"), () => StashRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_eatery"), () => EateryRequested?.Invoke())
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(m_Tavern.transform.position));
        }

        private void OnCommandBoardMouseUp()
        {
            MapRequested?.Invoke();
        }
    }
}