using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class RelicSaveDataMapper : Mapper<RelicSaveData, Relic>
    {
        private readonly IRelicRepository m_RelicRepository;

        public RelicSaveDataMapper(IRelicRepository relicRepository)
        {
            m_RelicRepository = relicRepository;
        }

        public override RelicSaveData ToData(Relic relic)
        {
            return new RelicSaveData
            {
                RelicId = relic.Id,
                Level = relic.Experience?.Level ?? 0,
                Experience = relic.Experience?.Current ?? 0,
            };
        }

        public override Relic ToEntity(RelicSaveData data)
        {
            if (data.RelicId == Relic.s_Empty.Id)
            {
                return Relic.s_Empty;
            }

            var relic = m_RelicRepository.Find(data.RelicId);

            relic.Construct(data.Level, data.Experience);

            return relic;
        }
    }
}