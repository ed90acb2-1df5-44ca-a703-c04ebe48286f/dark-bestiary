using System;
using System.Collections.Generic;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Views.Unity
{
    public class BuffSelectionView : View, IBuffSelectionView
    {
        public event Action<Behaviour> BuffSelected;

        [SerializeField] private TowerBuff m_BuffPrefab;
        [SerializeField] private Transform m_BuffContainer;
        [SerializeField] private Interactable m_ContinueButton;

        private TowerBuff m_Selected;

        public void Construct(List<Behaviour> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                var buff = Instantiate(m_BuffPrefab, m_BuffContainer);
                buff.Clicked += OnBuffClicked;
                buff.Construct(behaviour);
            }
        }

        protected override void OnInitialize()
        {
            m_ContinueButton.PointerClick += OnContinueButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            m_ContinueButton.PointerClick -= OnContinueButtonPointerClick;
        }

        private void OnContinueButtonPointerClick()
        {
            if (m_Selected == null)
            {
                return;
            }

            Hide();
        }

        private void OnBuffClicked(TowerBuff towerBuff)
        {
            m_Selected?.Deselect();
            m_Selected = towerBuff;
            m_Selected.Select();

            BuffSelected?.Invoke(m_Selected.Behaviour);
        }
    }
}