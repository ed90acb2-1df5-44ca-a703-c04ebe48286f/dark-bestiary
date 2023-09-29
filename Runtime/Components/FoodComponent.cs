using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Scenarios;

namespace DarkBestiary.Components
{
    // TODO: Remove this crap
    public class FoodComponent : Component
    {
        public IReadOnlyDictionary<FoodType, Food> Foods => m_Foods;

        private Dictionary<FoodType, Food> m_Foods;
        private BehavioursComponent m_Behaviours;

        public FoodComponent Construct()
        {
            m_Foods = new Dictionary<FoodType, Food>
            {
                {FoodType.Entree, null},
                {FoodType.Dessert, null},
                {FoodType.Drink, null},
            };

            return this;
        }

        protected override void OnInitialize()
        {
            m_Behaviours = GetComponent<BehavioursComponent>();

            Scenario.AnyScenarioExit += OnAnyScenarioExit;
        }

        protected override void OnTerminate()
        {
            Scenario.AnyScenarioExit -= OnAnyScenarioExit;
        }

        private void OnAnyScenarioExit(Scenario scenario)
        {
            foreach (var slot in m_Foods.ToList())
            {
                if (m_Foods[slot.Key] == null || m_Foods[slot.Key].Behaviour.IsApplied)
                {
                    continue;
                }

                m_Foods[slot.Key].Remove(m_Behaviours);
                m_Foods[slot.Key] = null;
            }
        }

        public void Apply(Dictionary<FoodType, Food> foods)
        {
            foreach (var food in m_Foods.Values)
            {
                food?.Remove(m_Behaviours);
            }

            foreach (var food in foods)
            {
                m_Foods[food.Key] = food.Value;
                m_Foods[food.Key]?.Apply(m_Behaviours);
            }
        }
    }
}