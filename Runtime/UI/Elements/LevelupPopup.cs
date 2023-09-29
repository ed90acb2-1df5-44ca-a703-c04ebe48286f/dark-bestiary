using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LevelupPopup : Singleton<LevelupPopup>
    {
        public event Action Hidden;

        [SerializeField]
        private Sprite m_AttributeRewardIcon;

        [SerializeField]
        private Sprite m_TalentPointRewardIcon;

        [SerializeField]
        private Sprite m_AttributePointRewardIcon;

        [SerializeField]
        private GameObject m_SpacePrefab;

        [SerializeField]
        private LevelupPopupSkillReward m_SkillRewardPrefab;

        [SerializeField]
        private LevelupPopupItemReward m_ItemRewardPrefab;

        [SerializeField]
        private LevelupPopupReward m_RewardPrefab;

        [SerializeField]
        private RectTransform m_RewardContainer;

        [SerializeField]
        private TextMeshProUGUI m_LevelText;

        [SerializeField]
        private Button m_OkayButton;

        private void Start()
        {
            Instance.Hide();
            m_OkayButton.onClick.AddListener(Hide);
        }

        public void Show(int level, List<Reward> rewards)
        {
            Clear();

            m_LevelText.text = $"{I18N.Instance.Get("ui_level")} {level}".ToUpper();

            AudioManager.Instance.PlayLevelUp();

            gameObject.SetActive(true);

            CreateTalentPoints(rewards.OfType<TalentPointsReward>().ToList());
            CreateAttributePoints(rewards.OfType<AttributePointsReward>().ToList());
            CreateAttributes(rewards.OfType<AttributesReward>().ToList());
            CreateProperties(rewards.OfType<PropertiesReward>().ToList());
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
                    Instantiate(m_RewardPrefab, m_RewardContainer).Construct(
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
                    var itemReward = Instantiate(m_ItemRewardPrefab, m_RewardContainer);
                    itemReward.Item = item;
                    itemReward.Construct(
                        Resources.Load<Sprite>(item.Icon),
                        I18N.Instance.Get("ui_received_item"),
                        item.ColoredName + (item.StackCount > 1 ? $" x{item.StackCount}" : "")
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
                Instantiate(m_RewardPrefab, m_RewardContainer).Construct(
                    m_TalentPointRewardIcon,
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
                Instantiate(m_RewardPrefab, m_RewardContainer).Construct(
                    m_AttributePointRewardIcon,
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
                    Instantiate(m_RewardPrefab, m_RewardContainer).Construct(
                        m_AttributeRewardIcon,
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
                    Instantiate(m_RewardPrefab, m_RewardContainer).Construct(
                        m_AttributeRewardIcon,
                        I18N.Instance.Get("ui_you_have_become_stronger"),
                        $"{attribute.Name} +{attribute.Base}"
                    );
                }
            }
        }

        private void CreateSpace()
        {
            Instantiate(m_SpacePrefab, m_RewardContainer);
        }

        private void Clear()
        {
            foreach (var child in m_RewardContainer.GetComponentsInChildren<Transform>())
            {
                if (m_RewardContainer.gameObject.Equals(child.gameObject))
                {
                    continue;
                }

                Destroy(child.gameObject);
            }
        }
    }
}