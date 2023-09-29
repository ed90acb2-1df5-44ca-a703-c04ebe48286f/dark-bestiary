using System;
using DarkBestiary.Data;
using DarkBestiary.Items;

namespace DarkBestiary.Map.Encounters
{
    public class VendorMapEncounter : IMapEncounter
    {
        private readonly MapEncounterData m_Data;
        private readonly MapEncounterRunner m_Context;

        private Action? m_OnSuccess;

        public VendorMapEncounter(MapEncounterData data, MapEncounterRunner context)
        {
            m_Data = data;
            m_Context = context;
        }

        public void Run(Action onSuccess, Action onFailure)
        {
            m_OnSuccess = onSuccess;

            m_Context.EquipmentViewController.View.RequiresConfirmationOnClose = true;
            m_Context.VendorViewController.View.RequiresConfirmationOnClose = true;
            m_Context.VendorViewController.View.Hidden += OnVendorViewHidden;

            Container.Instance.Instantiate<Loot>()
                .RollDropAsync(m_Data.LootId, assortment =>
                {
                    m_Context.VendorViewController.ClearBuyback();
                    m_Context.VendorViewController.ChangeAssortment(assortment);
                    m_Context.VendorViewController.View.Show();
                });
        }

        public void Cleanup()
        {
            m_Context.EquipmentViewController.View.RequiresConfirmationOnClose = false;
            m_Context.VendorViewController.View.RequiresConfirmationOnClose = false;
            m_Context.VendorViewController.View.Hidden -= OnVendorViewHidden;
        }

        private void OnVendorViewHidden()
        {
            m_OnSuccess?.Invoke();
        }
    }
}