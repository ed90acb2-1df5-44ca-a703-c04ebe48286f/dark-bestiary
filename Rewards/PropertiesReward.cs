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
        public List<Property> Properties { get; } = new List<Property>();

        private readonly PropertiesRewardData data;
        private readonly IPropertyRepository propertyRepository;

        public PropertiesReward(PropertiesRewardData data, IPropertyRepository propertyRepository)
        {
            this.data = data;
            this.propertyRepository = propertyRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            foreach (var attributeData in this.data.Properties)
            {
                Properties.Add(this.propertyRepository.Find(attributeData.PropertyId).Increase(attributeData.Amount));
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