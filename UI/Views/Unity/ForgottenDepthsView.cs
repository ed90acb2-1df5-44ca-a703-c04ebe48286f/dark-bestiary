using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Views.Unity
{
    public class ForgottenDepthsView : View, IForgottenDepthsView
    {
        public event Payload StartButtonClicked;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private CustomText textPrefab;
        [SerializeField] private Transform textContainer;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable startButton;

        private void Start()
        {
            this.closeButton.PointerClick += Hide;
            this.startButton.PointerClick += OnStartButtonPointerClick;
        }

        public void Construct(int depth, int monsterLevel, IReadOnlyList<Behaviour> behaviours)
        {
            this.titleText.text = $"{I18N.Instance.Translate("ui_depth")} {depth}\n<size=60%>{I18N.Instance.Translate("ui_monster_level")} {monsterLevel}</size>";

            foreach (var behaviour in behaviours)
            {
                Instantiate(this.textPrefab, this.textContainer).Text = behaviour.Description;
            }
        }

        private void OnStartButtonPointerClick()
        {
            StartButtonClicked?.Invoke();
        }
    }
}