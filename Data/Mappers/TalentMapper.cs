using System;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Talents;

namespace DarkBestiary.Data.Mappers
{
    public class TalentMapper : Mapper<TalentData, Talent>
    {
        private readonly IBehaviourRepository behaviourRepository;
        private readonly ITalentCategoryRepository talentCategoryRepository;

        public TalentMapper(IBehaviourRepository behaviourRepository,
            ITalentCategoryRepository talentCategoryRepository)
        {
            this.behaviourRepository = behaviourRepository;
            this.talentCategoryRepository = talentCategoryRepository;
        }

        public override TalentData ToData(Talent target)
        {
            throw new NotImplementedException();
        }

        public override Talent ToEntity(TalentData data)
        {
            return new Talent(data,
                this.behaviourRepository.Find(data.BehaviourId), this.talentCategoryRepository.Find(data.CategoryId));
        }
    }
}
