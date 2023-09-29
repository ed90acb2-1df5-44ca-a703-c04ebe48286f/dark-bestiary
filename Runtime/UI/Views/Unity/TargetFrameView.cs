using System.Collections.Generic;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Views.Unity
{
    public class TargetFrameView : View, ITargetFrameView
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_ChallengeRatingText;
        [SerializeField] private BehavioursPanel m_BehaviourFrame;
        [SerializeField] private AffixView m_AffixPrefab;
        [SerializeField] private Transform m_AffixContainer;
        [SerializeField] private HealthFrame m_HealthFrame;
        [SerializeField] private Color m_EnemyTextColor;
        [SerializeField] private Color m_AllyTextColor;

        private readonly List<AffixView> m_AffixViews = new();

        public void SetPoisoned(bool isPoisoned)
        {
            m_HealthFrame.SetPoisoned(isPoisoned);
        }

        public void CreateAffixes(List<Behaviour> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                var affixView = Instantiate(m_AffixPrefab, m_AffixContainer);
                affixView.Construct(behaviour);
                m_AffixViews.Add(affixView);
            }
        }

        public void ClearAffixes()
        {
            foreach (var affixView in m_AffixViews)
            {
                Destroy(affixView.gameObject);
            }

            m_AffixViews.Clear();
        }

        public void AddBehaviour(Behaviour behaviour)
        {
            m_BehaviourFrame.Add(behaviour);
        }

        public void RemoveBehaviour(Behaviour behaviour)
        {
            m_BehaviourFrame.Remove(behaviour);
        }

        public void ClearBehaviours()
        {
            m_BehaviourFrame.Clear();
        }

        public void ChangeNameText(string text, bool isEnemy)
        {
            m_NameText.text = text;
            m_NameText.color = isEnemy ? m_EnemyTextColor : m_AllyTextColor;
        }

        public void ChangeChallengeRatingText(string text)
        {
            m_ChallengeRatingText.text = text;
        }

        public void RefreshHealth(float currentHealth, float currentShield, float maximum)
        {
            m_HealthFrame.Refresh(currentHealth, currentShield, maximum);
        }
    }
}