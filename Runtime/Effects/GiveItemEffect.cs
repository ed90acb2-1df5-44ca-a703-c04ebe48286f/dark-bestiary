using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class GiveItemEffect : Effect
    {
        private readonly GiveItemEffectData m_Data;
        private readonly IItemRepository m_ItemRepository;

        public GiveItemEffect(GiveItemEffectData data, List<ValidatorWithPurpose> validators, IItemRepository itemRepository) : base(data, validators)
        {
            m_Data = data;
            m_ItemRepository = itemRepository;
        }

        protected override Effect New()
        {
            return new GiveItemEffect(m_Data, Validators, m_ItemRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var item = m_ItemRepository.FindOrFail(m_Data.ItemId).SetStack(m_Data.Count);
            target.GetComponent<InventoryComponent>().Pickup(item);
            TriggerFinished();
        }
    }
}