using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Modifiers;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.Components
{
    public class AttributesComponent : Component
    {
        public event Action<AttributesComponent, Attribute> AttributeChanged;
        public event Action<AttributesComponent> PointsChanged;

        private HealthComponent m_Health;
        private int m_Points;

        public int Points
        {
            get => m_Points;
            set
            {
                m_Points = value;
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
            m_Health = GetComponent<HealthComponent>();

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
            Points = (GetComponent<ExperienceComponent>().Experience.Level - 1) * Constants.c_AttributePointsPerLevel;

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
            var fraction = m_Health.HealthFraction;

            modifier.Entity = gameObject;
            Get(modifier.Attribute.Type).AddModifier(modifier);

            m_Health.Health = m_Health.HealthMax * fraction;
        }

        private void RemoveModifier(AttributeModifier modifier)
        {
            var fraction = m_Health.HealthFraction;

            Get(modifier.Attribute.Type).RemoveModifier(modifier);

            m_Health.Health = m_Health.HealthMax * fraction;
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