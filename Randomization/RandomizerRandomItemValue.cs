using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;

namespace DarkBestiary.Randomization
{
    public class RandomizerRandomItemValue : RandomizerItemValue
    {
        public static List<int> LearnedSkills { get; set; }

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

        public IRandomizerObject Clone()
        {
            var clone = new RandomizerRandomItemValue(this.data, this.itemRepository, this.itemCategoryRepository);
            clone.MonsterLevel = MonsterLevel;
            clone.Value = Value;
            clone.Table = Table;
            return clone;
        }

        public override void OnHit()
        {
            var itemCategory = this.itemCategoryRepository.Find(this.data.CategoryId);
            var unlockedRecipes = CharacterManager.Instance.Character.Data.UnlockedRecipes;
            var relics = CharacterManager.Instance.Character.Relics;

            bool Filter(ItemData item)
            {
                var itemDroppableFilter = ItemDroppableFilter(item);
                var monsterLevelFilter = MonsterLevelFilter(item);
                var gameModeFilter = GameModeFilter(item);
                var itemCategoryFilter = ItemCategoryFilter(item);
                var itemRarityFilter = ItemRarityFilter(item);
                var duplicateFilter = DuplicateFilter(item);

                var result = itemDroppableFilter &&
                       monsterLevelFilter &&
                       gameModeFilter &&
                       itemCategoryFilter &&
                       itemRarityFilter &&
                       duplicateFilter;

                return result;
            }

            bool ItemDroppableFilter(ItemData itemData)
            {
                return itemData.IsEnabled && itemData.Flags.HasFlag(ItemFlags.Droppable);
            }

            bool MonsterLevelFilter(ItemData item)
            {
                return Table?.IgnoreLevel == true || this.data.IgnoreLevel || Item.MatchDropByMonsterLevel(item, MonsterLevel);
            }

            bool GameModeFilter(ItemData item)
            {
                if (item.Flags.HasFlag(ItemFlags.CampaignOnly))
                {
                    return Game.Instance.IsCampaign;
                }

                if (item.Flags.HasFlag(ItemFlags.VisionsOnly))
                {
                    return Game.Instance.IsVisions;
                }

                if (itemCategory.Contains(item.BlueprintRecipeItemTypeId) || item.TypeId == Constants.ItemTypeIdIngredient)
                {
                    return Game.Instance.IsCampaign;
                }

                return true;
            }

            bool ItemCategoryFilter(ItemData item)
            {
                return itemCategory == null || itemCategory.Contains(item.TypeId) || itemCategory.Contains(item.BlueprintRecipeItemTypeId);
            }

            bool ItemRarityFilter(ItemData item)
            {
                return this.data.RarityId == 0 || item.RarityId == this.data.RarityId;
            }

            bool DuplicateFilter(ItemData item)
            {
                return !unlockedRecipes.Contains(item.BlueprintRecipeId) &&
                       !LearnedSkills.Contains(item.LearnSkillId) &&
                       relics.All(r => r.Id != item.UnlockRelicId);
            }

            Value = this.itemRepository.Random(1, Filter).FirstOrDefault();

            if (Value == null)
            {
                return;
            }

            base.OnHit();
        }
    }
}