using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LevelupPopup : Singleton<LevelupPopup>
    {
        public event Payload Hidden;

        [SerializeField] private Sprite attributeRewardIcon;
        [SerializeField] private Sprite talentPointRewardIcon;
        [SerializeField] private Sprite attributePointRewardIcon;
        [SerializeField] private GameObject spacePrefab;
        [SerializeField] private LevelupPopupSkillReward skillRewardPrefab;
        [SerializeField] private LevelupPopupItemReward itemRewardPrefab;
        [SerializeField] private LevelupPopupReward rewardPrefab;
        [SerializeField] private RectTransform rewardContainer;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button okayButton;

        private void Start()
        {
            Instance.Hide();
            this.okayButton.onClick.AddListener(Hide);
        }

        public void Show(int level, List<Reward> rewards)
        {
            Clear();

            this.levelText.text = $"{I18N.Instance.Get("ui_level")} {level}".ToUpper();

            AudioManager.Instance.PlayLevelUp();

            gameObject.SetActive(true);

            CreateTalentPoints(rewards.OfType<TalentPointsReward>().ToList());
            CreateAttributePoints(rewards.OfType<AttributePointsReward>().ToList());
            CreateAttributes(rewards.OfType<AttributesReward>().ToList());
            CreateProperties(rewards.OfType<PropertiesReward>().ToList());
            CreateSkills(rewards.OfType<RandomSkillsUnlockReward>().ToList());
            CreateItems(rewards.OfType<ItemsReward>().ToList());
            CreateCurrencies(rewards.OfType<CurrenciesReward>().ToList());
            CreateSpace();

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Hidden?.Invoke();
        }

        private void CreateCurrencies(IReadOnlyCollection<CurrenciesReward> rewards)
        {
            if (rewards.Count == 0)
            {
                return;
            }

            CreateSpace();

            foreach (var reward in rewards)
            {
                foreach (var currency in reward.Currencies)
                {
                    Instantiate(this.rewardPrefab, this.rewardContainer).Construct(
                        Resources.Load<Sprite>(currency.Icon),
                        I18N.Instance.Get("ui_received_currency"),
                        $"{currency.Name} x{currency.Amount}"
                    );
                }
            }
        }

        private void CreateItems(IReadOnlyCollection<ItemsReward> rewards)
        {
            if (rewards.Count == 0)
            {
                return;
            }

            CreateSpace();

            foreach (var reward in rewards)
            {
                foreach (var item in reward.Items)
                {
                    var itemReward = Instantiate(this.itemRewardPrefab, this.rewardContainer);
                    itemReward.Item = item;
                    itemReward.Construct(
                        Resources.Load<Sprite>(item.Icon),
                        I18N.Instance.Get("ui_received_item"),
                        item.ColoredName + (item.StackCount > 1 ? $" x{item.StackCount}" : "")
                    );
                }
            }
        }

        private void CreateSkills(IReadOnlyCollection<RandomSkillsUnlockReward> rewards)
        {
            if (rewards.Count == 0)
            {
                return;
            }

            CreateSpace();

            foreach (var reward in rewards)
            {
                foreach (var skill in reward.Skills)
                {
                    var skillReward = Instantiate(this.skillRewardPrefab, this.rewardContainer);
                    skillReward.Skill = skill;
                    skillReward.Construct(
                        Resources.Load<Sprite>(skill.Icon),
                        I18N.Instance.Get("ui_new_skill_unlocked"),
                        skill.Name
                    );
                }
            }
        }

        private void CreateTalentPoints(IReadOnlyCollection<TalentPointsReward> rewards)
        {
            if (rewards.Count == 0)
            {
                return;
            }

            CreateSpace();

            foreach (var reward in rewards)
            {
                Instantiate(this.rewardPrefab, this.rewardContainer).Construct(
                    this.talentPointRewardIcon,
                    I18N.Instance.Get("ui_new_talents_available"),
                    I18N.Instance.Get("ui_talent_points") + " +" + reward.Count
                );
            }
        }

        private void CreateAttributePoints(IReadOnlyCollection<AttributePointsReward> rewards)
        {
            if (rewards.Count == 0)
            {
                return;
            }

            CreateSpace();

            foreach (var reward in rewards)
            {
                Instantiate(this.rewardPrefab, this.rewardContainer).Construct(
                    this.attributePointRewardIcon,
                    I18N.Instance.Get("ui_attributes_improved"),
                    I18N.Instance.Get("ui_attribute_points") + " +" + reward.Count
                );
            }
        }

        private void CreateProperties(IReadOnlyCollection<PropertiesReward> rewards)
        {
            if (rewards.Count == 0)
            {
                return;
            }

            CreateSpace();

            foreach (var reward in rewards)
            {
                foreach (var property in reward.Properties)
                {
                    Instantiate(this.rewardPrefab, this.rewardContainer).Construct(
                        this.attributeRewardIcon,
                        I18N.Instance.Get("ui_you_have_become_stronger"),
                        $"{property.Name} +{property.Base}"
                    );
                }
            }
        }

        private void CreateAttributes(IReadOnlyCollection<AttributesReward> attributesRewards)
        {
            if (attributesRewards.Count == 0)
            {
                return;
            }

            CreateSpace();

            foreach (var attributesReward in attributesRewards)
            {
                foreach (var attribute in attributesReward.Attributes)
                {
                    Instantiate(this.rewardPrefab, this.rewardContainer).Construct(
                        this.attributeRewardIcon,
                        I18N.Instance.Get("ui_you_have_become_stronger"),
                        $"{attribute.Name} +{attribute.Base}"
                    );
                }
            }
        }

        private void CreateSpace()
        {
            Instantiate(this.spacePrefab, this.rewardContainer);
        }

        private void Clear()
        {
            foreach (var child in this.rewardContainer.GetComponentsInChildren<Transform>())
            {
                if (this.rewardContainer.gameObject.Equals(child.gameObject))
                {
                    continue;
                }

                Destroy(child.gameObject);
            }
        }
    }
}