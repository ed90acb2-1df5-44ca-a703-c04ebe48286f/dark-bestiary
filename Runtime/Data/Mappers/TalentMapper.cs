using System;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Talents;

namespace DarkBestiary.Data.Mappers
{
    public class TalentMapper : Mapper<TalentData, Talent>
    {
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly ITalentCategoryRepository m_TalentCategoryRepository;

        public TalentMapper(IBehaviourRepository behaviourRepository,
            ITalentCategoryRepository talentCategoryRepository)
        {
            m_BehaviourRepository = behaviourRepository;
            m_TalentCategoryRepository = talentCategoryRepository;
        }

        public override TalentData ToData(Talent target)
        {
            throw new NotImplementedException();
        }

        public override Talent ToEntity(TalentData data)
        {
            return new Talent(data,
                m_BehaviourRepository.Find(data.BehaviourId), m_TalentCategoryRepository.Find(data.CategoryId));
        }
    }
}