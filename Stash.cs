using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class Stash : IInitializable
    {
        public const int TabCount = 5;
        public const int SlotsPerTab = 150;

        public static Stash Instance { get; private set; }

        public InventoryComponent[] Inventories { get; } = new InventoryComponent[TabCount];

        private readonly IFileReader reader;
        private readonly ItemSaveDataMapper itemSaveDataMapper;
        private readonly StorageId storageId;

        private Character character;

        public Stash(IFileReader reader, ItemSaveDataMapper itemSaveDataMapper, StorageId storageId)
        {
            Instance = this;

            var stash = new GameObject("Stash");
            stash.transform.position = new Vector3(-400, 0, 0);

            for (var i = 0; i < TabCount; i++)
            {
                Inventories[i] = stash.AddComponent<InventoryComponent>();
                Inventories[i].Construct(SlotsPerTab);
                Inventories[i].IsIngredientsStash = i == TabCount - 1;
            }

            this.reader = reader;
            this.itemSaveDataMapper = itemSaveDataMapper;
            this.storageId = storageId;
        }

        public InventoryComponent GetIngredientsInventory()
        {
            return Inventories.Last();
        }

        public void Initialize()
        {
            try
            {
                var data = this.reader.Read<StashData>(GetDataPath()) ?? new StashData();
                var inventory = Inventories[0];
                var itemIndex = 0;

                for (var i = 0; i < data.Items.Count; i++)
                {
                    if (i % SlotsPerTab == 0)
                    {
                        inventory = Inventories[i / SlotsPerTab];
                        itemIndex = 0;
                    }

                    var item = this.itemSaveDataMapper.ToEntity(data.Items[i]);

                    try
                    {
                        inventory.PickupDoNotStack(item, inventory.Items[itemIndex]);
                    }
                    catch (Exception)
                    {
                        inventory.PickupDoNotStack(item);
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

            GameState.AnyGameStateExit += OnAnyGameStateExit;
            CharacterManager.CharacterSelected += OnCharacterSelected;
            Application.quitting += OnApplicationQuitting;
        }

        private string GetDataPath()
        {
            return Environment.PersistentDataPath + $"/{this.storageId}/stash.save";
        }

        private void OnAnyGameStateExit(GameState gameState)
        {
            Save();
        }

        private void OnCharacterSelected(Character character)
        {
            this.character = character;

            foreach (var inventory in Inventories)
            {
                inventory.Owner = this.character.Entity;

                foreach (var item in inventory.Items)
                {
                    item.ChangeOwner(this.character.Entity);
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
                items.AddRange(inventory.Items.Select(this.itemSaveDataMapper.ToData).ToList());
            }

            this.reader.Write(
                new StashData {Items = items},
                GetDataPath()
            );
        }
    }
}