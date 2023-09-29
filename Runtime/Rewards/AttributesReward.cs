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
        public List<Attribute> Attributes { get; } = new();

        private readonly AttributesRewardData m_Data;
        private readonly IAttributeRepository m_AttributeRepository;

        public AttributesReward(AttributesRewardData data, IAttributeRepository attributeRepository)
        {
            m_Data = data;
            m_AttributeRepository = attributeRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            foreach (var attributeData in m_Data.Attributes)
            {
                Attributes.Add(m_AttributeRepository.Find(attributeData.AttributeId).Increase(attributeData.Amount));
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