using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Items
{
    public class ItemSet
    {
        public int Id { get; }
        public I18NString Name { get; }

        // TODO: Circular dependency ducttape
        public List<Item> Items =>
            m_Items ?? (m_Items = Container.Instance.Resolve<IItemRepository>().Find(m_ItemIds));

        public Dictionary<int, List<Behaviour>> Behaviours { get; }

        private List<Item> m_Items;
        private readonly List<int> m_ItemIds;

        public ItemSet(ItemSetData data, Dictionary<int, List<Behaviour>> behaviours)
        {
            Id = data.Id;
            Name = I18N.Instance.Get(data.NameKey);
            Behaviours = behaviours;

            m_ItemIds = data.Items;
        }
    }
}