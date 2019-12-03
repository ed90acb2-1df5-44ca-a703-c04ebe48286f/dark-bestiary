using DarkBestiary.Currencies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CurrencyView : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField] private Interactable hover;

        private Currency currency;

        public void Initialize(Currency currency)
        {
            this.image.sprite = Resources.Load<Sprite>(currency.Icon);

            this.currency = currency;
            this.currency.Changed += OnCurrencyChanged;

            if (this.hover != null)
            {
                this.hover.PointerEnter += OnPointerEnter;
                this.hover.PointerExit += OnPointerExit;
            }

            OnCurrencyChanged(this.currency);
        }

        public void Terminate()
        {
            this.currency.Changed -= OnCurrencyChanged;
        }

        protected override void OnDespawn()
        {
            Terminate();
        }

        private void OnCurrencyChanged(Currency currency)
        {
            this.text.text = currency.Amount.ToString();
        }

        private void OnPointerEnter()
        {
            Tooltip.Instance.Show(this.currency.Name, GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            Tooltip.Instance.Hide();
        }
    }
}