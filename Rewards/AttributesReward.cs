using System.Collections.Generic;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using UnityEngine;

namespace DarkBestiary.Rewards
{
    public class AttributesReward : Reward
    {
        public List<Attribute> Attributes { get; } = new List<Attribute>();

        private readonly AttributesRewardData data;
        private readonly IAttributeRepository attributeRepository;

        public AttributesReward(AttributesRewardData data, IAttributeRepository attributeRepository)
        {
            this.data = data;
            this.attributeRepository = attributeRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            foreach (var attributeData in this.data.Attributes)
            {
                Attributes.Add(this.attributeRepository.Find(attributeData.AttributeId).Increase(attributeData.Amount));
            }
        }

        protected override void OnClaim(GameObject entity)
        {
            var attributes = entity.GetComponent<AttributesComponent>();

            foreach (var attribute in Attributes)
            {
                attributes.Get(attribute.Type).Increase(attribute.Base);
            }
        }
    }
}