using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class ItemSetMapper : Mapper<ItemSetData, ItemSet>
    {
        private readonly IBehaviourRepository behaviourRepository;

        public ItemSetMapper(IBehaviourRepository behaviourRepository)
        {
            this.behaviourRepository = behaviourRepository;
        }

        public override ItemSetData ToData(ItemSet entity)
        {
            throw new System.NotImplementedException();
        }

        public override ItemSet ToEntity(ItemSetData data)
        {
            var behaviours = new Dictionary<int, List<Behaviour>>();

            foreach (var behaviourData in data.Behaviours)
            {
                behaviours.Add(behaviourData.ItemCount, this.behaviourRepository.Find(behaviourData.Behaviours));
            }

            return new ItemSet(data, behaviours);
        }
    }
}