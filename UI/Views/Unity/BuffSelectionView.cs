using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Views.Unity
{
    public class BuffSelectionView : View, IBuffSelectionView
    {
        public event Payload<Behaviour> BuffSelected;

        [SerializeField] private TowerBuff buffPrefab;
        [SerializeField] private Transform buffContainer;
        [SerializeField] private Interactable continueButton;

        private TowerBuff selected;

        public void Construct(List<Behaviour> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                var buff = Instantiate(this.buffPrefab, this.buffContainer);
                buff.Clicked += OnBuffClicked;
                buff.Construct(behaviour);
            }
        }

        protected override void OnInitialize()
        {
            this.continueButton.PointerClick += OnContinueButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            this.continueButton.PointerClick -= OnContinueButtonPointerClick;
        }

        private void OnContinueButtonPointerClick()
        {
            if (this.selected == null)
            {
                return;
            }

            Hide();
        }

        private void OnBuffClicked(TowerBuff towerBuff)
        {
            this.selected?.Deselect();
            this.selected = towerBuff;
            this.selected.Select();

            BuffSelected?.Invoke(this.selected.Behaviour);
        }
    }
}