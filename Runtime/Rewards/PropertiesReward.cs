using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Properties;
using UnityEngine;

namespace DarkBestiary.Rewards
{
    public class PropertiesReward : Reward
    {
        public List<Property> Properties { get; } = new();

        private readonly PropertiesRewardData m_Data;
        private readonly IPropertyRepository m_PropertyRepository;

        public PropertiesReward(PropertiesRewardData data, IPropertyRepository propertyRepository)
        {
            m_Data = data;
            m_PropertyRepository = propertyRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            foreach (var attributeData in m_Data.Properties)
            {
                Properties.Add(m_PropertyRepository.Find(attributeData.PropertyId).Increase(attributeData.Amount));
            }
        }

        protected override void OnClaim(GameObject entity)
        {
            var properties = entity.GetComponent<PropertiesComponent>();

            foreach (var property in Properties)
            {
                properties.Get(property.Type).Increase(property.Base);
            }
        }
    }
}