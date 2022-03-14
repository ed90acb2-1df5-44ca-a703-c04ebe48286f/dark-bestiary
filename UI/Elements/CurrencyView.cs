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

        private void Start()
        {
            this.hover.PointerEnter += OnPointerEnter;
            this.hover.PointerExit += OnPointerExit;
        }

        private void OnDestroy()
        {
            this.hover.PointerEnter -= OnPointerEnter;
            this.hover.PointerExit -= OnPointerExit;
        }

        public void Initialize(Currency currency)
        {
            if (this.currency != null)
            {
                this.currency.Changed -= OnCurrencyChanged;
            }

            this.currency = currency;
            this.currency.Changed += OnCurrencyChanged;

            this.image.sprite = Resources.Load<Sprite>(this.currency.Icon);
            OnCurrencyChanged(this.currency);
        }

        public void Terminate()
        {
            if (this.currency == null)
            {
                return;
            }

            this.currency.Changed -= OnCurrencyChanged;
            this.currency = null;
        }

        protected override void OnDespawn()
        {
            Terminate();
        }

        private void OnCurrencyChanged(Currency currency)
        {
            gameObject.SetActive(currency.Amount > 0);
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