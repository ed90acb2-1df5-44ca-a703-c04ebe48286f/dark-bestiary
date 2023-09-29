using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class Stash : IInitializable
    {
        public const int c_TabCount = 5;
        public const int c_SlotsPerTab = 150;

        public static Stash Instance { get; private set; }

        public InventoryComponent[] Inventories { get; } = new InventoryComponent[c_TabCount];

        private readonly IFileReader m_Reader;
        private readonly ItemSaveDataMapper m_ItemSaveDataMapper;
        private readonly StorageId m_StorageId;

        private Character m_Character;

        public Stash(IFileReader reader, ItemSaveDataMapper itemSaveDataMapper, StorageId storageId)
        {
            Instance = this;

            var stash = new GameObject("Stash");
            stash.transform.position = new Vector3(-400, 0, 0);

            for (var i = 0; i < c_TabCount; i++)
            {
                Inventories[i] = stash.AddComponent<InventoryComponent>();
                Inventories[i].Construct(c_SlotsPerTab);
                Inventories[i].IsIngredientsStash = i == c_TabCount - 1;
            }

            m_Reader = reader;
            m_ItemSaveDataMapper = itemSaveDataMapper;
            m_StorageId = storageId;
        }

        public InventoryComponent GetIngredientsInventory()
        {
            return Inventories.Last();
        }

        public void Initialize()
        {
            try
            {
                var data = m_Reader.Read<StashData>(GetDataPath()) ?? new StashData();
                var inventory = Inventories[0];
                var itemIndex = 0;

                for (var i = 0; i < data.Items.Count; i++)
                {
                    if (i % c_SlotsPerTab == 0)
                    {
                        inventory = Inventories[i / c_SlotsPerTab];
                        itemIndex = 0;
                    }

                    try
                    {
                        var item = m_ItemSaveDataMapper.ToEntity(data.Items[i]);

                        try
                        {
                            inventory.PickupDoNotStack(item, inventory.Items[itemIndex]);
                        }
                        catch (Exception)
                        {
                            inventory.PickupDoNotStack(item);
                        }
                    }
                    catch (Exception _)
                    {
                        continue;
                    }

                    itemIndex++;
                }
            }
            catch (Exception exception)
            {
                Timer.Instance.WaitForFixedUpdate(() =>
                {
                    UiErrorFrame.Instance.ShowException(exception);
                    CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
                });

                throw;
            }

            Game.Instance.SceneSwitched += OnSceneSwitched;
            Game.Instance.CharacterSwitched += OnCharacterSwitched;
            Application.quitting += OnApplicationQuitting;
        }

        private string GetDataPath()
        {
            return Environment.s_PersistentDataPath + $"/{m_StorageId}/stash.save";
        }

        private void OnSceneSwitched()
        {
            Save();
        }

        private void OnCharacterSwitched()
        {
            m_Character = Game.Instance.Character;

            foreach (var inventory in Inventories)
            {
                inventory.Owner = m_Character.Entity;

                foreach (var item in inventory.Items)
                {
                    item.ChangeOwner(m_Character.Entity);
                }
            }
        }

        private void OnApplicationQuitting()
        {
            Save();
        }

        private void Save()
        {
            var items = new List<ItemSaveData>();

            foreach (var inventory in Inventories)
            {
                items.AddRange(inventory.Items.Select(m_ItemSaveDataMapper.ToData).ToList());
            }

            m_Reader.Write(new StashData { Items = items }, GetDataPath());
        }
    }
}