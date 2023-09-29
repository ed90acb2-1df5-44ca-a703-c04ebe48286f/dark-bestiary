using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class RelicMapper : Mapper<RelicData, Relic>
    {
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly IRarityRepository m_RarityRepository;

        public RelicMapper(IBehaviourRepository behaviourRepository, IRarityRepository rarityRepository)
        {
            m_BehaviourRepository = behaviourRepository;
            m_RarityRepository = rarityRepository;
        }

        public override RelicData ToData(Relic entity)
        {
            throw new System.NotImplementedException();
        }

        public override Relic ToEntity(RelicData data)
        {
            return new Relic(
                data, m_RarityRepository.Find(data.RarityId), m_BehaviourRepository.Find(data.BehaviourId));
        }
    }
}