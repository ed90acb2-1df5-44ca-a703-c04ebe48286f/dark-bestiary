using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Messaging;
using DarkBestiary.Modifiers;

namespace DarkBestiary.Components
{
    public class AttributesComponent : Component
    {
        public event Payload<AttributesComponent, Attribute> AttributeChanged;
        public event Payload<AttributesComponent> PointsChanged;

        private HealthComponent health;
        private int points;

        public int Points
        {
            get => this.points;
            set
            {
                this.points = value;
                PointsChanged?.Invoke(this);
            }
        }

        public Dictionary<AttributeType, Attribute> Attributes { get; private set; }

        public AttributesComponent Construct(IEnumerable<Attribute> attributes)
        {
            Attributes = new Dictionary<AttributeType, Attribute>();

            foreach (var attribute in attributes)
            {
                Attributes.Add(attribute.Type, attribute);
            }

            return this;
        }

        protected override void OnInitialize()
        {
            this.health = GetComponent<HealthComponent>();

            var properties = GetComponent<PropertiesComponent>();

            foreach (var attribute in Attributes.Values)
            {
                attribute.Changed += OnAttributeChanged;
                attribute.Initialize(properties);
            }
        }

        protected override void OnTerminate()
        {
            foreach (var attribute in Attributes.Values)
            {
                attribute.Changed -= OnAttributeChanged;
            }
        }

        public float GetMaxAttribute()
        {
            return Attributes.Values.Where(a => a.IsPrimary).Max(a => a.Value());
        }

        public float GetAverageAttribute()
        {
            return Attributes.Values.Where(a => a.IsPrimary).Average(a => a.Value());
        }

        private void OnAttributeChanged(Attribute attribute)
        {
            AttributeChanged?.Invoke(this, attribute);
        }

        public void ResetPoints()
        {
            Points = (GetComponent<ExperienceComponent>().Experience.Level - 1) * Constants.AttributePointsPerLevel;

            foreach (var attribute in Attributes.Values)
            {
                attribute.ResetPoints();
            }
        }

        public void AddPoint(Attribute attribute, int points)
        {
            if (points > Points)
            {
                points = Points;
            }

            Points -= points;
            attribute.AddPoint(points);
            PointsChanged?.Invoke(this);
        }

        public void SubtractPoint(Attribute attribute, int points)
        {
            if (points > attribute.Points)
            {
                points = attribute.Points;
            }

            Points += points;
            attribute.SubtractPoint(points);
            PointsChanged?.Invoke(this);
        }

        public Attribute Get(AttributeType type)
        {
            return Attributes[type];
        }

        public Attribute Get(int id)
        {
            return Attributes.Values.First(attribute => attribute.Id == id);
        }

        private void AddModifier(AttributeModifier modifier)
        {
            var fraction = this.health.HealthFraction;

            modifier.Entity = gameObject;
            Get(modifier.Attribute.Type).AddModifier(modifier);

            this.health.Health = this.health.HealthMax * fraction;
        }

        private void RemoveModifier(AttributeModifier modifier)
        {
            var fraction = this.health.HealthFraction;

            Get(modifier.Attribute.Type).RemoveModifier(modifier);

            this.health.Health = this.health.HealthMax * fraction;
        }

        public void ApplyModifiers(IEnumerable<AttributeModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                AddModifier(modifier);
            }
        }

        public void RemoveModifiers(IEnumerable<AttributeModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                RemoveModifier(modifier);
            }
        }
    }
}