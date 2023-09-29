using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Managers
{
    public class Mailbox : IInitializable
    {
        public static Mailbox Instance { get; private set; }

        public event Action Updated;

        public IReadOnlyCollection<Item> Items => m_Items;

        private readonly ItemSaveDataMapper m_Mapper;
        private readonly IFileReader m_Reader;
        private readonly StorageId m_StorageId;

        private List<Item> m_Items;

        public Mailbox(ItemSaveDataMapper mapper, IFileReader reader, StorageId storageId)
        {
            m_Mapper = mapper;
            m_Reader = reader;
            m_StorageId = storageId;
        }

        public void Initialize()
        {
            var data = new List<ItemSaveData>();

            try
            {
                data = m_Reader.Read<List<ItemSaveData>>(GetDataPath()) ?? new List<ItemSaveData>();
            }
            catch (Exception exception)
            {
                // ignored
            }

            m_Items = data.Where(d => d.ItemId > 0).Select(d => m_Mapper.ToEntity(d)).ToList();

            Application.quitting += OnApplicationQuitting;

            Instance = this;
        }

        private void OnApplicationQuitting()
        {
            var data = m_Items.Select(i => m_Mapper.ToData(i)).ToList();

            m_Reader.Write(data, GetDataPath());
        }

        public void SendMail(Item item)
        {
            m_Items.Add(item);
            Updated?.Invoke();
        }

        public void SendMail(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                SendMail(item);
            }
        }

        public void RemoveMail(Item item)
        {
            m_Items.Remove(item);
            Updated?.Invoke();
        }

        public void TakeMail(Item item, GameObject entity)
        {
            RemoveMail(item);

            entity.GetComponent<InventoryComponent>().Pickup(item);

            Updated?.Invoke();
        }

        public void TakeAll(GameObject entity)
        {
            var inventory = entity.GetComponent<InventoryComponent>();

            while (true)
            {
                if (m_Items.Count == 0 || inventory.GetFreeSlotCount() == 0)
                {
                    break;
                }

                inventory.Pickup(m_Items[0]);
                m_Items.Remove(m_Items[0]);
            }

            Updated?.Invoke();
        }

        private string GetDataPath()
        {
            return Environment.s_PersistentDataPath + $"/{m_StorageId}/mailbox.save";
        }
    }
}