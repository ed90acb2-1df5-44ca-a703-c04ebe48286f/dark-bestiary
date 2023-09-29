using DarkBestiary.Data.Repositories;
using DarkBestiary.Masteries;

namespace DarkBestiary.Data.Mappers
{
    public class MasterySaveDataMapper : Mapper<MasterySaveData, Mastery>
    {
        private readonly IMasteryRepository m_MasteryRepository;

        public MasterySaveDataMapper(IMasteryRepository masteryRepository)
        {
            m_MasteryRepository = masteryRepository;
        }

        public override MasterySaveData ToData(Mastery mastery)
        {
            return new MasterySaveData
            {
                MasteryId = mastery.Id,
                Level = mastery.Experience?.Level ?? 0,
                Experience = mastery.Experience?.Current ?? 0,
            };
        }

        public override Mastery ToEntity(MasterySaveData data)
        {
            var relic = m_MasteryRepository.Find(data.MasteryId);

            relic.Construct(data.Level, data.Experience);

            return relic;
        }
    }
}