using DarkBestiary.Managers;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class EndTurnButton : Interactable
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private TextMeshProUGUI m_HotkeyText;
        [SerializeField] private Image m_Globe;

        
        [Header("Colors")]
        [SerializeField] private Color m_NormalGlobeColor;
        [SerializeField] private Color m_NormalTextColor;
        [SerializeField] private Color m_DisabledGlobeColor;
        [SerializeField] private Color m_DisabledTextColor;

        private void Start()
        {
            m_HotkeyText.text = EnumTranslator.Translate(KeyBindings.Get(KeyType.EndTurn));
        }

        protected override void OnActivate()
        {
            m_Globe.color = m_NormalGlobeColor;
            m_Text.color = m_NormalTextColor;
            m_Text.text = I18N.Instance.Get("ui_end_turn");
        }

        protected override void OnDeactivate()
        {
            m_Globe.color = m_DisabledGlobeColor;
            m_Text.color = m_DisabledTextColor;
            m_Text.text = I18N.Instance.Get("ui_enemy_turn");
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