using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class SkillSetMapper : Mapper<SkillSetData, SkillSet>
    {
        private readonly IBehaviourRepository behaviourRepository;

        public SkillSetMapper(IBehaviourRepository behaviourRepository)
        {
            this.behaviourRepository = behaviourRepository;
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
                behaviours.Add(behaviourData.SkillCount, this.behaviourRepository.Find(behaviourData.Behaviours));
            }

            return new SkillSet(data, behaviours);
        }
    }
}