using DarkBestiary.Data;

namespace DarkBestiary.Components
{
    public class ArchetypeComponent : Component
    {
        private ArchetypeData m_Data;

        public ArchetypeComponent Construct(ArchetypeData data)
        {
            m_Data = data;
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

            foreach (var archetypeAttributeData in m_Data.Attributes)
            {
                var attribute = attributes.Get(archetypeAttributeData.AttributeId);

                attribute.SetArchetypeBonus(
                    Curve.Evaluate(
                        unit.Level * (0.5f + unit.ChallengeRating * 0.5f),
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

            foreach (var archetypePropertyData in m_Data.Properties)
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
    }
}