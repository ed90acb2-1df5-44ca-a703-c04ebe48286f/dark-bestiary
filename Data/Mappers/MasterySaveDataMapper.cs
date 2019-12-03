using DarkBestiary.Data.Repositories;
using DarkBestiary.Masteries;

namespace DarkBestiary.Data.Mappers
{
    public class MasterySaveDataMapper : Mapper<MasterySaveData, Mastery>
    {
        private readonly IMasteryRepository masteryRepository;

        public MasterySaveDataMapper(IMasteryRepository masteryRepository)
        {
            this.masteryRepository = masteryRepository;
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
            var relic = this.masteryRepository.Find(data.MasteryId);

            relic.Construct(data.Level, data.Experience);

            return relic;
        }
    }
}