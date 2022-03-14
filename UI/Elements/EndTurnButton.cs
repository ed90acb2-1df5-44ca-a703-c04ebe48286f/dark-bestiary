using DarkBestiary.Managers;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class EndTurnButton : Interactable
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI hotkeyText;
        [SerializeField] private Image globe;

        [Header("Colors")]
        [SerializeField] private Color normalGlobeColor;
        [SerializeField] private Color normalTextColor;
        [SerializeField] private Color disabledGlobeColor;
        [SerializeField] private Color disabledTextColor;

        private void Start()
        {
            this.hotkeyText.text = EnumTranslator.Translate(KeyBindings.Get(KeyType.EndTurn));
        }

        protected override void OnActivate()
        {
            this.globe.color = this.normalGlobeColor;
            this.text.color = this.normalTextColor;
            this.text.text = I18N.Instance.Get("ui_end_turn");
        }

        protected override void OnDeactivate()
        {
            this.globe.color = this.disabledGlobeColor;
            this.text.color = this.disabledTextColor;
            this.text.text = I18N.Instance.Get("ui_enemy_turn");
        }

        private void Update()
        {
            if (!Active || UIManager.Instance.IsAnyFullscreenUiOpen())
            {
                return;
            }

            if (Input.GetKeyUp(KeyBindings.Get(KeyType.EndTurn)))
            {
                TriggerMouseUp();
            }
        }
    }
}