using DarkBestiary.Currencies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CurrencyView : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Image m_Image;
        [SerializeField] private Interactable m_Hover;

        private Currency m_Currency;

        private void Start()
        {
            m_Hover.PointerEnter += OnPointerEnter;
            m_Hover.PointerExit += OnPointerExit;
        }

        private void OnDestroy()
        {
            m_Hover.PointerEnter -= OnPointerEnter;
            m_Hover.PointerExit -= OnPointerExit;
        }

        public void Initialize(Currency currency)
        {
            if (m_Currency != null)
            {
                m_Currency.Changed -= OnCurrencyChanged;
            }

            m_Currency = currency;
            m_Currency.Changed += OnCurrencyChanged;

            m_Image.sprite = Resources.Load<Sprite>(m_Currency.Icon);
            OnCurrencyChanged(m_Currency);
        }

        public void Terminate()
        {
            if (m_Currency == null)
            {
                return;
            }

            m_Currency.Changed -= OnCurrencyChanged;
            m_Currency = null;
        }

        protected override void OnDespawn()
        {
            Terminate();
        }

        private void OnCurrencyChanged(Currency currency)
        {
            gameObject.SetActive(currency.Amount > 0);
            m_Text.text = currency.Amount.ToString();
        }

        private void OnPointerEnter()
        {
            Tooltip.Instance.Show(m_Currency.Name, GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            Tooltip.Instance.Hide();
        }
    }
}