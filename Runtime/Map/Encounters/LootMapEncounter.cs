using System;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.Map.Encounters
{
    public class LootMapEncounter : IMapEncounter
    {
        private readonly MapEncounterData m_Data;

        private Action? m_OnSuccess;

        public LootMapEncounter(MapEncounterData data)
        {
            m_Data = data;
        }

        public void Run(Action onSuccess, Action onFailure)
        {
            m_OnSuccess = onSuccess;

            Container.Instance.Instantiate<Loot>().RollDropAsync(m_Data.LootId, items =>
            {
                var container = new Item(0, I18N.Instance.Get("item_treasure_chest_name"), new ItemData { Icon = "Sprites/Icons/Items/icon_reward_chest_02" }, null);
                ContainerWindow.Instance.Show(container, items);
                ContainerWindow.Instance.Hidden += OnContainerWindowHidden;

                Game.Instance.Character.Entity.GetComponent<InventoryComponent>().Pickup(items);
            });
        }

        public void Cleanup()
        {
            ContainerWindow.Instance.Hidden -= OnContainerWindowHidden;
        }

        private void OnContainerWindowHidden()
        {
            m_OnSuccess?.Invoke();
        }
    }
}