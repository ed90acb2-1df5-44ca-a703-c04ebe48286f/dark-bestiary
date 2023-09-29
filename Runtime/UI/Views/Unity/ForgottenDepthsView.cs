using System;
using System.Collections.Generic;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Views.Unity
{
    public class ForgottenDepthsView : View, IForgottenDepthsView
    {
        public event Action StartButtonClicked;

        [SerializeField] private TextMeshProUGUI m_TitleText;
        [SerializeField] private CustomText m_TextPrefab;
        [SerializeField] private Transform m_TextContainer;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_StartButton;

        private void Start()
        {
            m_CloseButton.PointerClick += Hide;
            m_StartButton.PointerClick += OnStartButtonPointerClick;
        }

        public void Construct(int depth, int monsterLevel, IReadOnlyList<Behaviour> behaviours)
        {
            m_TitleText.text = $"{I18N.Instance.Translate("ui_depth")} {depth}\n<size=60%>{I18N.Instance.Translate("ui_monster_level")} {monsterLevel}</size>";

            foreach (var behaviour in behaviours)
            {
                Instantiate(m_TextPrefab, m_TextContainer).Text = behaviour.Description;
            }
        }

        private void OnStartButtonPointerClick()
        {
            StartButtonClicked?.Invoke();
        }
    }
}