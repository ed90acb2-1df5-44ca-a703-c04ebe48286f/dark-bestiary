using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class RelicMapper : Mapper<RelicData, Relic>
    {
        private readonly IBehaviourRepository behaviourRepository;
        private readonly IRarityRepository rarityRepository;

        public RelicMapper(IBehaviourRepository behaviourRepository, IRarityRepository rarityRepository)
        {
            this.behaviourRepository = behaviourRepository;
            this.rarityRepository = rarityRepository;
        }

        public override RelicData ToData(Relic entity)
        {
            throw new System.NotImplementedException();
        }

        public override Relic ToEntity(RelicData data)
        {
            return new Relic(
                data, this.rarityRepository.Find(data.RarityId), this.behaviourRepository.Find(data.BehaviourId));
        }
    }
}