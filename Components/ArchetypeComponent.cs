using System;
using DarkBestiary.Attributes;
using DarkBestiary.Data;
using UnityEngine;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.Components
{
    public class ArchetypeComponent : Component
    {
        private ArchetypeData data;

        public ArchetypeComponent Construct(ArchetypeData data)
        {
            this.data = data;
            return this;
        }

        protected override void OnInitialize()
        {
            var unit = GetComponent<UnitComponent>();
            unit.LevelChanged += OnUnitLevelChanged;

            ApplyAttributes(unit);
            ApplyProperties(unit);
        }

        protected override void OnTerminate()
        {
            GetComponent<UnitComponent>().LevelChanged -= OnUnitLevelChanged;
        }

        private void OnUnitLevelChanged(UnitComponent unit)
        {
            ApplyAttributes(unit);
        }

        private void ApplyAttributes(UnitComponent unit)
        {
            var attributes = GetComponent<AttributesComponent>();

            foreach (var archetypeAttributeData in this.data.Attributes)
            {
                var attribute = attributes.Get(archetypeAttributeData.AttributeId);

                attribute.SetArchetypeBonus(
                    Curve.Evaluate(
                        unit.Level * GetChallengeRatingMultiplier(attribute, unit.ChallengeRating),
                        archetypeAttributeData.Min,
                        archetypeAttributeData.Max,
                        archetypeAttributeData.CurveType
                    )
                );
            }
        }

        private void ApplyProperties(UnitComponent unit)
        {
            var properties = GetComponent<PropertiesComponent>();

            foreach (var archetypePropertyData in this.data.Properties)
            {
                properties.Get(archetypePropertyData.PropertyId).SetArchetypeBonus(
                    Curve.Evaluate(
                        unit.Level,
                        archetypePropertyData.Min,
                        archetypePropertyData.Max,
                        archetypePropertyData.CurveType
                    )
                );
            }
        }

        private static float GetChallengeRatingMultiplier(Attribute attribute, int challengeRating)
        {
            if (challengeRating == 1)
            {
                return 1;
            }

            float multiplier;

            if (attribute.Type == AttributeType.Constitution)
            {
                multiplier = Mathf.Pow(Math.Max(1, challengeRating), 1.15f);
            }
            else
            {
                multiplier = Mathf.Max(1, Mathf.Pow(challengeRating, 0.5f));
            }

            return multiplier;
        }
    }
}