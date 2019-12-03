using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;

namespace DarkBestiary.Randomization
{
    public class RandomizerRandomItemValue : RandomizerItemValue
    {
        public int MonsterLevel { get; set; }

        private readonly IItemCategoryRepository itemCategoryRepository;
        private readonly IItemRepository itemRepository;
        private readonly LootItemData data;

        public RandomizerRandomItemValue(LootItemData data, IItemRepository itemRepository,
            IItemCategoryRepository itemCategoryRepository) : base(null, data)
        {
            this.data = data;
            this.itemRepository = itemRepository;
            this.itemCategoryRepository = itemCategoryRepository;
        }

        public override int GetHashCode()
        {
            return Value == null ? base.GetHashCode() : Value.Id;
        }

        public override bool Equals(object @object)
        {
            if (@object is RandomizerRandomItemValue random)
            {
                return random.Value != null && Value != null && Value.Id == random.Value.Id;
            }

            return @object?.Equals(this) ?? false;
        }

        public override void OnHit()
        {
            var itemCategory = this.itemCategoryRepository.Find(this.data.CategoryId);
            var unlockedRecipes = CharacterManager.Instance.Character.Data.UnlockedRecipes;
            var relics = CharacterManager.Instance.Character.Relics;

            Value = this.itemRepository
                .Random(1, item =>
                {
                    return item.Flags.HasFlag(ItemFlags.Droppable) &&
                           item.RarityId == this.data.RarityId &&
                           Item.MatchDropByMonsterLevel(item.Level, MonsterLevel) &&
                           (itemCategory == null ||
                            itemCategory.Contains(item.TypeId) ||
                            itemCategory.Contains(item.BlueprintRecipeItemTypeId)) &&
                           !unlockedRecipes.Contains(item.BlueprintRecipeId) &&
                           relics.All(r => r.Id != item.UnlockRelicId);
                })
                .FirstOrDefault();

            if (Value == null)
            {
                return;
            }

            base.OnHit();
        }
    }
}