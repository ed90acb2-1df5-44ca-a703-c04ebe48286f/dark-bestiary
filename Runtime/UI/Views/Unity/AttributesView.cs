using System;
using System.Collections.Generic;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.UI.Views.Unity
{
    public class AttributesView : View, IAttributesView
    {
        public event Action<Attribute> AddPoint;
        public event Action<Attribute> AddMultiplePoints;
        public event Action<Attribute> SubtractPoint;
        public event Action<Attribute> SubtractMultiplePoints;
        public event Action Reset;

        [SerializeField] private TextMeshProUGUI m_PointsText;
        [SerializeField] private AttributeRow m_AttributeRowPrefab;
        [SerializeField] private Transform m_AttributeRowContainer;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_ResetButton;
        [SerializeField] private Interactable m_ApplyButton;

        private List<AttributeRow> m_AttributeRows;

        public void Construct(List<Attribute> attributes)
        {
            m_AttributeRows = new List<AttributeRow>();

            foreach (var attribute in attributes)
            {
                var attributeRow = Instantiate(m_AttributeRowPrefab, m_AttributeRowContainer);
                attributeRow.PlusButtonClicked += OnPlusButtonClicked;
                attributeRow.MinusButtonClicked += OnMinusButtonClicked;
                attributeRow.Construct(attribute);

                m_AttributeRows.Add(attributeRow);
            }

            m_CloseButton.PointerClick += Hide;
            m_ApplyButton.PointerClick += Hide;
            m_ResetButton.PointerClick += OnResetButtonDown;
        }

        protected override void OnTerminate()
        {
            m_CloseButton.PointerClick -= Hide;
            m_ApplyButton.PointerClick -= Hide;
            m_ResetButton.PointerClick -= OnResetButtonDown;

            foreach (var attributeRow in m_AttributeRows)
            {
                attributeRow.PlusButtonClicked -= OnPlusButtonClicked;
                attributeRow.MinusButtonClicked -= OnMinusButtonClicked;
            }
        }

        public void EnableSpendButtons()
        {
            foreach (var attributeRow in m_AttributeRows)
            {
                attributeRow.EnablePlusButton();
            }
        }

        public void DisableSpendButtons()
        {
            foreach (var attributeRow in m_AttributeRows)
            {
                attributeRow.DisablePlusButton();
            }
        }

        public void Refresh()
        {
            foreach (var attributeRow in m_AttributeRows)
            {
                attributeRow.Refresh();
            }
        }

        public void SetUnspentPoints(int amount)
        {
            m_PointsText.text = I18N.Instance.Get("ui_unspent_points_x").ToString(amount);
        }

        private void OnPlusButtonClicked(AttributeRow row)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                AddMultiplePoints?.Invoke(row.Attribute);
            }
            else
            {
                AddPoint?.Invoke(row.Attribute);
            }
        }

        private void OnMinusButtonClicked(AttributeRow row)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                SubtractMultiplePoints?.Invoke(row.Attribute);
            }
            else
            {
                SubtractPoint?.Invoke(row.Attribute);
            }
        }

        private void OnResetButtonDown()
        {
            Reset?.Invoke();
        }
    }
}