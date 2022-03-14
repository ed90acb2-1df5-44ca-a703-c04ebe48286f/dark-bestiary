using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Views;
using DarkBestiary.Visions;

namespace DarkBestiary.UI.Controllers
{
    public class VisionsSummaryViewController : ViewController<IVisionSummaryView>
    {
        private readonly IItemRepository itemRepository;
        private readonly CharacterManager characterManager;
        private readonly bool isSuccess;
        private readonly List<KeyValuePair<string, string>> summary;

        public VisionsSummaryViewController(IVisionSummaryView view, IItemRepository itemRepository,
            CharacterManager characterManager, Summary summary, bool isSuccess) : base(view)
        {
            this.itemRepository = itemRepository;
            this.characterManager = characterManager;
            this.isSuccess = isSuccess;
            this.summary = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_visions_completed"), summary.VisionsCompleted.ToString()),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_rounds"), summary.Rounds.ToString()),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_monsters_slain"), summary.MonstersSlain.ToString()),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_bosses_slain"), summary.BossesSlain.ToString()),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_skills_learned"), summary.Skills.ToString()),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_legendaries_obtained"), summary.Legendaries.ToString()),

                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_damage_dealt"), summary.DamageDealt.ToString("N0")),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_highest_damage_dealt"), summary.HighestDamageDealt.ToString("N0")),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_damage_taken"), summary.DamageTaken.ToString("N0")),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_highest_damage_taken"), summary.HighestDamageTaken.ToString("N0")),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_healing_taken"), summary.HealingTaken.ToString("N0")),
                new KeyValuePair<string, string>(I18N.Instance.Translate("ui_summary_highest_healing_taken"), summary.HighestHealingTaken.ToString("N0")),
            };
        }

        protected override void OnInitialize()
        {
            var rewards = GetRewards();

            Mailbox.Instance.SendMail(rewards);

            View.CompleteButtonClicked += OnCompleteButtonClicked;
            View.Construct(rewards, this.summary);
            View.SetSuccess(this.isSuccess);
        }

        protected override void OnTerminate()
        {
            View.CompleteButtonClicked -= OnCompleteButtonClicked;
        }

        private List<Item> GetRewards()
        {
            var illusory = this.characterManager.Character.Entity.GetComponent<InventoryComponent>().Items.Where(i => i.IsMarkedAsIllusory).ToList();

            foreach (var item in illusory)
            {
                item.IsMarkedAsIllusory = false;

                if (!this.isSuccess)
                {
                    item.SharpeningLevel = 0;
                }
            }

            var rewards = new List<Item>();
            rewards.AddRange(illusory);

            if (this.isSuccess)
            {
                rewards.Add(this.itemRepository.Find(Constants.ItemIdSphereOfVisions));
                rewards.Add(this.itemRepository.Find(Constants.ItemIdIllusorySubstance));
                rewards.AddRange(this.itemRepository.Random(VisionManager.Instance.GetRewardCount(), VisionItemFilter));
            }

            return rewards;
        }

        private static bool VisionItemFilter(ItemData item)
        {
            return item.RarityId == Constants.ItemRarityIdVision && item.IsEnabled &&
                   (ItemCategory.Armor.Contains(item.TypeId) ||
                    ItemCategory.Weapon.Contains(item.TypeId) ||
                    ItemCategory.Jewelry.Contains(item.TypeId));
        }

        private void OnCompleteButtonClicked()
        {
            Terminate();
        }
    }
}