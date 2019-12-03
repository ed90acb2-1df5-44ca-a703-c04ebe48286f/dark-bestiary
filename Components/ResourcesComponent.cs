using System.Collections.Generic;
using DarkBestiary.Attributes;
using DarkBestiary.Behaviours;
using DarkBestiary.Effects;
using DarkBestiary.Exceptions;
using DarkBestiary.GameStates;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Components
{
    public class ResourcesComponent : Component
    {
        public static event Payload<Resource> AnyResourceChanged;

        public event Payload<Resource> RageChanged;
        public event Payload<Resource> ActionPointsChanged;

        private readonly Dictionary<ResourceType, Resource> resources = new Dictionary<ResourceType, Resource>();

        private AttributesComponent attributes;
        private PropertiesComponent properties;

        protected override void OnInitialize()
        {
            this.attributes = GetComponent<AttributesComponent>();
            this.properties = GetComponent<PropertiesComponent>();

            var actionPoints = new Resource(gameObject, I18N.Instance.Get("resource_action_points"), ResourceType.ActionPoint, 10, 10);
            actionPoints.Changed += OnActionPointsChanged;

            var rage = new Resource(gameObject, I18N.Instance.Get("resource_rage"), ResourceType.Rage, 0, 100);
            rage.Changed += OnRageChanged;

            this.resources.Add(ResourceType.ActionPoint, actionPoints);
            this.resources.Add(ResourceType.Rage, rage);

            var actionPointsProperty = this.properties.Get(PropertyType.MaximumActionPoints);
            actionPointsProperty.Changed += OnMaxActionPointsChanged;

            OnMaxActionPointsChanged(actionPointsProperty);

            HealthComponent.AnyEntityHealed += OnEntityHealed;
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
            BehavioursComponent.AnyBehaviourApplied += OnAnyBehaviourApplied;

            Episode.AnyEpisodeStarted += OnAnyEpisodeInitialized;
            GameState.AnyGameStateEnter += OnAnyGameStateEnter;
            CombatEncounter.AnyCombatEnded += OnAnyCombatEnded;
            CombatEncounter.AnyCombatRoundStarted += OnAnyCombatRoundStarted;
        }

        protected override void OnTerminate()
        {
            this.resources[ResourceType.ActionPoint].Changed -= OnActionPointsChanged;
            this.resources[ResourceType.Rage].Changed -= OnRageChanged;
            this.properties.Get(PropertyType.MaximumActionPoints).Changed -= OnMaxActionPointsChanged;

            HealthComponent.AnyEntityHealed -= OnEntityHealed;
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
            BehavioursComponent.AnyBehaviourApplied -= OnAnyBehaviourApplied;

            Episode.AnyEpisodeStarted -= OnAnyEpisodeInitialized;
            GameState.AnyGameStateEnter -= OnAnyGameStateEnter;
            CombatEncounter.AnyCombatEnded -= OnAnyCombatEnded;
            CombatEncounter.AnyCombatRoundStarted -= OnAnyCombatRoundStarted;
        }

        private void OnAnyBehaviourApplied(Behaviour behaviour)
        {
            if (behaviour is ShieldBehaviour shieldBehaviour)
            {
                Restore(ResourceType.Rage, shieldBehaviour.Amount / CalculatePower() * 100);
            }
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            if (data.Healer != gameObject || data.Target != gameObject ||
                data.Healing.IsRegeneration() || data.Healing.IsVampirism())
            {
                return;
            }

            Restore(ResourceType.Rage, data.Healing.Amount / CalculatePower() * 100);
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Attacker != gameObject)
            {
                return;
            }

            Restore(ResourceType.Rage, data.Damage.Amount / CalculatePower() * 100);
        }

        private float CalculatePower()
        {
            return (this.attributes.Get(AttributeType.Defense).Value() +
                    this.attributes.Get(AttributeType.Resistance).Value() +
                    this.properties.Get(PropertyType.Thorns).Value() +
                    this.properties.Get(PropertyType.SpellPower).Value() +
                    this.properties.Get(PropertyType.AttackPower).Value()) * 30;
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
            var actionPoints = this.resources[ResourceType.ActionPoint];
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

        private void OnAnyGameStateEnter(GameState gameState)
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
            return this.resources.ContainsKey(type) ? this.resources[type] : null;
        }

        public void Restore()
        {
            foreach (var resource in this.resources)
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