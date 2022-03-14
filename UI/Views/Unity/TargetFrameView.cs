using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Views.Unity
{
    public class TargetFrameView : View, ITargetFrameView
    {
        public event Payload UnsummonButtonClicked;

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI challengeRatingText;
        [SerializeField] private BehavioursPanel behaviourFrame;
        [SerializeField] private AffixView affixPrefab;
        [SerializeField] private Transform affixContainer;
        [SerializeField] private HealthFrame healthFrame;
        [SerializeField] private Interactable unsummonButton;
        [SerializeField] private Color enemyTextColor;
        [SerializeField] private Color allyTextColor;

        private readonly List<AffixView> affixViews = new List<AffixView>();

        private void Start()
        {
            this.unsummonButton.PointerClick += () => UnsummonButtonClicked?.Invoke();
        }

        public void SetKillButtonActive(bool active)
        {
            this.unsummonButton.gameObject.SetActive(active);
        }

        public void SetPoisoned(bool isPoisoned)
        {
            this.healthFrame.SetPoisoned(isPoisoned);
        }

        public void CreateAffixes(List<Behaviour> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                var affixView = Instantiate(this.affixPrefab, this.affixContainer);
                affixView.Construct(behaviour);
                this.affixViews.Add(affixView);
            }
        }

        public void ClearAffixes()
        {
            foreach (var affixView in this.affixViews)
            {
                Destroy(affixView.gameObject);
            }

            this.affixViews.Clear();
        }

        public void AddBehaviour(Behaviour behaviour)
        {
            this.behaviourFrame.Add(behaviour);
        }

        public void RemoveBehaviour(Behaviour behaviour)
        {
            this.behaviourFrame.Remove(behaviour);
        }

        public void ClearBehaviours()
        {
            this.behaviourFrame.Clear();
        }

        public void ChangeNameText(string text, bool isEnemy)
        {
            this.nameText.text = text;
            this.nameText.color = isEnemy ? this.enemyTextColor : this.allyTextColor;
        }

        public void ChangeChallengeRatingText(string text)
        {
            this.challengeRatingText.text = text;
        }

        public void RefreshHealth(float currentHealth, float currentShield, float maximum)
        {
            this.healthFrame.Refresh(currentHealth, currentShield, maximum);
        }
    }
}