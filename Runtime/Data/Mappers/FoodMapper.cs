using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Data.Mappers
{
    public class FoodMapper : Mapper<FoodData, Food>
    {
        private readonly IBehaviourRepository m_BehaviourRepository;

        public FoodMapper(IBehaviourRepository behaviourRepository)
        {
            m_BehaviourRepository = behaviourRepository;
        }

        public override FoodData ToData(Food entity)
        {
            throw new System.NotImplementedException();
        }

        public override Food ToEntity(FoodData data)
        {
            return new Food(data, m_BehaviourRepository.Find(data.BehaviourId));
        }
    }
}