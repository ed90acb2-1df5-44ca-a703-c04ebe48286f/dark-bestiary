using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class RelicSaveDataMapper : Mapper<RelicSaveData, Relic>
    {
        private readonly IRelicRepository relicRepository;

        public RelicSaveDataMapper(IRelicRepository relicRepository)
        {
            this.relicRepository = relicRepository;
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
            if (data.RelicId == Relic.Empty.Id)
            {
                return Relic.Empty;
            }

            var relic = this.relicRepository.Find(data.RelicId);

            relic.Construct(data.Level, data.Experience);

            return relic;
        }
    }
}