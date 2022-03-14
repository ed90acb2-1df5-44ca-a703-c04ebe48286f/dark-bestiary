using System.Collections.Generic;
using DarkBestiary.Attributes;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class AttributesView : View, IAttributesView
    {
        public event Payload<Attribute> AddPoint;
        public event Payload<Attribute> AddMultiplePoints;
        public event Payload<Attribute> SubtractPoint;
        public event Payload<Attribute> SubtractMultiplePoints;
        public event Payload Reset;

        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private AttributeRow attributeRowPrefab;
        [SerializeField] private Transform attributeRowContainer;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable resetButton;
        [SerializeField] private Interactable applyButton;

        private List<AttributeRow> attributeRows;

        public void Construct(List<Attribute> attributes)
        {
            this.attributeRows = new List<AttributeRow>();

            foreach (var attribute in attributes)
            {
                var attributeRow = Instantiate(this.attributeRowPrefab, this.attributeRowContainer);
                attributeRow.PlusButtonClicked += OnPlusButtonClicked;
                attributeRow.MinusButtonClicked += OnMinusButtonClicked;
                attributeRow.Construct(attribute);

                this.attributeRows.Add(attributeRow);
            }

            this.closeButton.PointerClick += Hide;
            this.applyButton.PointerClick += Hide;
            this.resetButton.PointerClick += OnResetButtonDown;
        }

        protected override void OnTerminate()
        {
            this.closeButton.PointerClick -= Hide;
            this.applyButton.PointerClick -= Hide;
            this.resetButton.PointerClick -= OnResetButtonDown;

            foreach (var attributeRow in this.attributeRows)
            {
                attributeRow.PlusButtonClicked -= OnPlusButtonClicked;
                attributeRow.MinusButtonClicked -= OnMinusButtonClicked;
            }
        }

        public void EnableSpendButtons()
        {
            foreach (var attributeRow in this.attributeRows)
            {
                attributeRow.EnablePlusButton();
            }
        }

        public void DisableSpendButtons()
        {
            foreach (var attributeRow in this.attributeRows)
            {
                attributeRow.DisablePlusButton();
            }
        }

        public void Refresh()
        {
            foreach (var attributeRow in this.attributeRows)
            {
                attributeRow.Refresh();
            }
        }

        public void SetUnspentPoints(int amount)
        {
            this.pointsText.text = I18N.Instance.Get("ui_unspent_points_x").ToString(amount);
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