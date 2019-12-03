using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Data.Mappers
{
    public class FoodMapper : Mapper<FoodData, Food>
    {
        private readonly IBehaviourRepository behaviourRepository;

        public FoodMapper(IBehaviourRepository behaviourRepository)
        {
            this.behaviourRepository = behaviourRepository;
        }

        public override FoodData ToData(Food entity)
        {
            throw new System.NotImplementedException();
        }

        public override Food ToEntity(FoodData data)
        {
            return new Food(data, this.behaviourRepository.Find(data.BehaviourId));
        }
    }
}