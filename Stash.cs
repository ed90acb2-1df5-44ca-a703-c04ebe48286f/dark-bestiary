using System;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Managers;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class Stash : IInitializable
    {
        public static Stash Instance { get; private set; }

        public InventoryComponent Inventory { get; }

        private readonly IFileReader reader;
        private readonly ItemSaveDataMapper itemSaveDataMapper;
        private readonly StorageId storageId;

        private Character character;

        public Stash(IFileReader reader, ItemSaveDataMapper itemSaveDataMapper, StorageId storageId)
        {
            Instance = this;

            Inventory = new GameObject("Stash").AddComponent<InventoryComponent>();
            Inventory.transform.position = new Vector3(-400, 0, 0);
            Inventory.Construct(160);

            this.reader = reader;
            this.itemSaveDataMapper = itemSaveDataMapper;
            this.storageId = storageId;
        }

        public void Initialize()
        {
            var data = new StashData();

            try
            {
                data = this.reader.Read<StashData>(GetDataPath()) ?? new StashData();
            }
            catch (Exception exception)
            {
                // ignored
            }

            var items = this.itemSaveDataMapper.ToEntity(data.Items)
                .Where(item => !item.IsEmpty)
                .ToList();

            Inventory.Pickup(items);

            CharacterManager.CharacterSelected += OnCharacterSelected;

            Application.quitting += OnApplicationQuitting;
        }

        private string GetDataPath()
        {
            return Application.persistentDataPath + $"/{this.storageId}/stash.save";
        }

        private void OnCharacterSelected(Character character)
        {
            this.character = character;

            Inventory.Owner = this.character.Entity;

            foreach (var item in Inventory.Items)
            {
                item.ChangeOwner(this.character.Entity);
            }
        }

        private void OnApplicationQuitting()
        {
            this.reader.Write(
                new StashData {Items = Inventory.Items.Select(this.itemSaveDataMapper.ToData).ToList()},
                GetDataPath());
        }
    }
}