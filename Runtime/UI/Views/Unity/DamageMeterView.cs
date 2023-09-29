using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class DamageMeterView : View, IDamageMeterView
    {
        public enum FilterType
        {
            DamageDoneAlly,
            HealingDoneAlly,
            DamageTakenAlly,
            HealingTakenAlly,
            DamageDoneEnemy,
            HealingDoneEnemy,
            DamageTakenEnemy,
            HealingTakenEnemy,
        }

        [SerializeField] private DamageMeterRow m_RowPrefab;
        [SerializeField] private Transform m_RowContainer;
        [SerializeField] private TMP_Dropdown m_Dropdown;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_ResetButton;

        private MonoBehaviourPool<DamageMeterRow> m_Pool;
        private DamageMeterViewController m_Controller;

        protected override void OnInitialize()
        {
            m_Pool = MonoBehaviourPool<DamageMeterRow>.Factory(m_RowPrefab, m_RowContainer);

            m_Dropdown.options = new List<TMP_Dropdown.OptionData>
            {
                new(EnumTranslator.Translate(FilterType.DamageDoneAlly)),
                new(EnumTranslator.Translate(FilterType.HealingDoneAlly)),
                new(EnumTranslator.Translate(FilterType.DamageTakenAlly)),
                new(EnumTranslator.Translate(FilterType.HealingTakenAlly)),
                new(EnumTranslator.Translate(FilterType.DamageDoneEnemy)),
                new(EnumTranslator.Translate(FilterType.HealingDoneEnemy)),
                new(EnumTranslator.Translate(FilterType.DamageTakenEnemy)),
                new(EnumTranslator.Translate(FilterType.HealingTakenEnemy)),
            };

            m_Dropdown.onValueChanged.AddListener(OnFilterChanged);
            m_Dropdown.value = (int) FilterType.DamageDoneAlly;
            m_Dropdown.RefreshShownValue();

            m_CloseButton.PointerClick += Hide;
            m_ResetButton.PointerClick += OnResetButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            m_Pool.Clear();

            m_CloseButton.PointerClick -= Hide;
            m_ResetButton.PointerClick -= OnResetButtonPointerClick;
        }

        public void Construct(DamageMeterViewController controller)
        {
            m_Controller = controller;
            m_Controller.DamageMetersUpdated += OnDamageMetersUpdated;
            OnDamageMetersUpdated();
        }

        private void OnResetButtonPointerClick()
        {
            m_Controller.Reset();
        }

        private void OnFilterChanged(int value)
        {
            OnDamageMetersUpdated();
        }

        private void OnDamageMetersUpdated()
        {
            m_Pool.DespawnAll();

            var totalAmount = m_Controller.DamageMeters.Where(Filter).Sum(GetTotalAmount);

            foreach (var damageMeter in m_Controller.DamageMeters.Where(Filter).OrderByDescending(GetTotalAmount))
            {
                var (amount, amountPerTurn) = GetTotalAndPerTurnAmount(damageMeter);
                var fraction = totalAmount > 0 ? amount / totalAmount : 0;

                m_Pool.Spawn().UpdateProperties(
                    damageMeter.Name, $"{amount.ToString("F0")} ({amountPerTurn.ToString("F0")}, {(fraction * 100).ToString("F0")}%)", fraction, damageMeter.IsAlive);
            }
        }

        private bool Filter(DamageMeter damageMeter)
        {
            var filter = (FilterType) m_Dropdown.value;

            if (filter is
                FilterType.DamageDoneAlly or
                FilterType.HealingDoneAlly or
                FilterType.DamageTakenAlly or
                FilterType.HealingTakenAlly)
            {
                return damageMeter.IsAlly;
            }

            return damageMeter.IsAlly == false;
        }

        private (float amount, float amountPerTurn) GetTotalAndPerTurnAmount(DamageMeter damageMeter)
        {
            var filter = (FilterType) m_Dropdown.value;

            switch (filter)
            {
                case FilterType.DamageDoneAlly:
                case FilterType.DamageDoneEnemy:
                    return (damageMeter.DamageDone, (int) damageMeter.DamageDonePerTurn);

                case FilterType.HealingDoneAlly:
                case FilterType.HealingDoneEnemy:
                    return (damageMeter.HealingDone, (int) damageMeter.HealingDonePerTurn);

                case FilterType.HealingTakenAlly:
                case FilterType.HealingTakenEnemy:
                    return (damageMeter.HealingTaken, (int) damageMeter.HealingTakenPerTurn);

                case FilterType.DamageTakenAlly:
                case FilterType.DamageTakenEnemy:
                    return (damageMeter.DamageTaken, (int) damageMeter.DamageTakenPerTurn);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float GetTotalAmount(DamageMeter damageMeter)
        {
            var (amount, _) = GetTotalAndPerTurnAmount(damageMeter);
            return amount;
        }
    }
}