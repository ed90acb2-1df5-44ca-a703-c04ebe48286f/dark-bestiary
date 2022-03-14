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
        private readonly GiveItemEffectData data;
        private readonly IItemRepository itemRepository;

        public GiveItemEffect(GiveItemEffectData data, List<ValidatorWithPurpose> validators, IItemRepository itemRepository) : base(data, validators)
        {
            this.data = data;
            this.itemRepository = itemRepository;
        }

        protected override Effect New()
        {
            return new GiveItemEffect(this.data, this.Validators, this.itemRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var item = this.itemRepository.FindOrFail(this.data.ItemId).SetStack(this.data.Count);
            target.GetComponent<InventoryComponent>().Pickup(item);
            TriggerFinished();
        }
    }
}