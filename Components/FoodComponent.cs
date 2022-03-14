using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Scenarios;

namespace DarkBestiary.Components
{
    // TODO: Remove this crap
    public class FoodComponent : Component
    {
        public IReadOnlyDictionary<FoodType, Food> Foods => this.foods;

        private Dictionary<FoodType, Food> foods;
        private BehavioursComponent behaviours;

        public FoodComponent Construct()
        {
            this.foods = new Dictionary<FoodType, Food>
            {
                {FoodType.Entree, null},
                {FoodType.Dessert, null},
                {FoodType.Drink, null},
            };

            return this;
        }

        protected override void OnInitialize()
        {
            this.behaviours = GetComponent<BehavioursComponent>();

            Scenario.AnyScenarioExit += OnAnyScenarioExit;
        }

        protected override void OnTerminate()
        {
            Scenario.AnyScenarioExit -= OnAnyScenarioExit;
        }

        private void OnAnyScenarioExit(Scenario scenario)
        {
            foreach (var slot in this.foods.ToList())
            {
                if (this.foods[slot.Key] == null || this.foods[slot.Key].Behaviour.IsApplied)
                {
                    continue;
                }

                this.foods[slot.Key].Remove(this.behaviours);
                this.foods[slot.Key] = null;
            }
        }

        public void Apply(Dictionary<FoodType, Food> foods)
        {
            foreach (var food in this.foods.Values)
            {
                food?.Remove(this.behaviours);
            }

            foreach (var food in foods)
            {
                this.foods[food.Key] = food.Value;
                this.foods[food.Key]?.Apply(this.behaviours);
            }
        }
    }
}