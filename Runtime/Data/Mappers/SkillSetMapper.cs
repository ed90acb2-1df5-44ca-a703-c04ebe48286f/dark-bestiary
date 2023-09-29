using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class SkillSetMapper : Mapper<SkillSetData, SkillSet>
    {
        private readonly IBehaviourRepository m_BehaviourRepository;

        public SkillSetMapper(IBehaviourRepository behaviourRepository)
        {
            m_BehaviourRepository = behaviourRepository;
        }

        public override SkillSetData ToData(SkillSet entity)
        {
            throw new System.NotImplementedException();
        }

        public override SkillSet ToEntity(SkillSetData data)
        {
            var behaviours = new Dictionary<int, List<Behaviour>>();

            foreach (var behaviourData in data.Behaviours)
            {
                behaviours.Add(behaviourData.SkillCount, m_BehaviourRepository.Find(behaviourData.Behaviours));
            }

            return new SkillSet(data, behaviours);
        }
    }
}