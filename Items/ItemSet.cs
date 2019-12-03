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
            this.items ?? (this.items = Container.Instance.Resolve<IItemRepository>().Find(this.itemIds));

        public Dictionary<int, List<Behaviour>> Behaviours { get; }

        private List<Item> items;
        private readonly List<int> itemIds;

        public ItemSet(ItemSetData data, Dictionary<int, List<Behaviour>> behaviours)
        {
            Id = data.Id;
            Name = I18N.Instance.Get(data.NameKey);
            Behaviours = behaviours;

            this.itemIds = data.Items;
        }
    }
}