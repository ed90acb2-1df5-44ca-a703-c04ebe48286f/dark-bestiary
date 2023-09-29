using System;
using System.Collections.Generic;
using DarkBestiary.Exceptions;
using DarkBestiary.Properties;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class ResourcesComponent : Component
    {
        public static event Action<Resource>? AnyResourceChanged;

        public event Action<Resource>? RageChanged;
        public event Action<Resource>? ActionPointsChanged;

        private readonly Dictionary<ResourceType, Resource> m_Resources = new();

        private AttributesComponent m_Attributes = null!;
        private PropertiesComponent m_Properties = null!;

        protected override void OnInitialize()
        {
            m_Attributes = GetComponent<AttributesComponent>();
            m_Properties = GetComponent<PropertiesComponent>();

            var actionPointsProperty = m_Properties.Get(PropertyType.MaximumActionPoints);
            actionPointsProperty.Changed += OnMaxActionPointsChanged;

            var actionPoints = new Resource(gameObject, I18N.Instance.Get("resource_action_points"), ResourceType.ActionPoint, actionPointsProperty.Value(), actionPointsProperty.Value());
            actionPoints.Changed += OnActionPointsChanged;

            var rage = new Resource(gameObject, I18N.Instance.Get("resource_rage"), ResourceType.Rage, 0, 100);
            rage.Changed += OnRageChanged;

            m_Resources.Add(ResourceType.ActionPoint, actionPoints);
            m_Resources.Add(ResourceType.Rage, rage);

            Skill.AnySkillUsed += OnAnySkillUsed;
            Episode.AnyEpisodeStarted += OnAnyEpisodeInitialized;
            Game.Instance.SceneSwitched += OnSceneSwitched;
            CombatEncounter.AnyCombatEnded += OnAnyCombatEnded;
            CombatEncounter.AnyCombatRoundStarted += OnAnyCombatRoundStarted;
        }

        protected override void OnTerminate()
        {
            m_Resources[ResourceType.ActionPoint].Changed -= OnActionPointsChanged;
            m_Resources[ResourceType.Rage].Changed -= OnRageChanged;
            m_Properties.Get(PropertyType.MaximumActionPoints).Changed -= OnMaxActionPointsChanged;

            Skill.AnySkillUsed -= OnAnySkillUsed;
            Episode.AnyEpisodeStarted -= OnAnyEpisodeInitialized;
            Game.Instance.SceneSwitched -= OnSceneSwitched;
            CombatEncounter.AnyCombatEnded -= OnAnyCombatEnded;
            CombatEncounter.AnyCombatRoundStarted -= OnAnyCombatRoundStarted;
        }

        private void OnAnySkillUsed(SkillUseEventData payload)
        {
            if (payload.Caster != gameObject || CombatEncounter.Active == null)
            {
                return;
            }

            var rage = Mathf.Max(1, payload.Skill.GetCost(ResourceType.ActionPoint)) * 2;
            rage *= m_Properties.Get(PropertyType.RageGeneration).Value();

            Restore(ResourceType.Rage, rage);
        }

        private void OnRageChanged(Resource resource)
        {
            RageChanged?.Invoke(resource);
            AnyResourceChanged?.Invoke(resource);
        }

        private void OnActionPointsChanged(Resource resource)
        {
            ActionPointsChanged?.Invoke(resource);
            AnyResourceChanged?.Invoke(resource);
        }

        private void OnMaxActionPointsChanged(Property property)
        {
            var actionPoints = m_Resources[ResourceType.ActionPoint];
            actionPoints.MaxAmount = property.Value();
            actionPoints.Amount = actionPoints.Amount;
        }

        private void OnAnyCombatEnded(CombatEncounter combat)
        {
            Restore(ResourceType.ActionPoint);
        }

        private void OnAnyEpisodeInitialized(Episode payload)
        {
            Restore(ResourceType.ActionPoint);
        }

        private void OnSceneSwitched()
        {
            Restore(ResourceType.ActionPoint);
            Get(ResourceType.Rage).Amount = 0;
        }

        private void OnAnyCombatRoundStarted(CombatEncounter combat)
        {
            Restore(ResourceType.ActionPoint);
        }

        public Resource Get(ResourceType type)
        {
            return m_Resources.ContainsKey(type) ? m_Resources[type] : null;
        }

        public void Restore()
        {
            foreach (var resource in m_Resources)
            {
                Restore(resource.Key);
            }
        }

        public void Restore(ResourceType resourceType)
        {
            var resource = Get(resourceType);

            if (resource == null)
            {
                return;
            }

            Restore(resource.Type, resource.MaxAmount);
        }

        public void Restore(ResourceType type, float amount)
        {
            var resource = Get(type);

            if (resource == null || float.IsNaN(amount))
            {
                return;
            }

            resource.Amount += amount;
        }

        public bool HasEnough(ResourceType type, float amount)
        {
            return Get(type)?.Amount >= amount;
        }

        public bool HasEnough(Dictionary<ResourceType, float> dictionary)
        {
            foreach (var amount in dictionary)
            {
                if (!HasEnough(amount.Key, amount.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public void Consume(Dictionary<ResourceType, float> costs)
        {
            foreach (var cost in costs)
            {
                Consume(cost.Key, cost.Value);
            }
        }

        public void Consume(ResourceType type, float amount)
        {
            var resource = Get(type);

            if (!HasEnough(type, amount))
            {
                throw new InsufficientResourceException(resource);
            }

            resource.Amount = Mathf.Clamp(resource.Amount - amount, 0, resource.MaxAmount);
        }
    }
}