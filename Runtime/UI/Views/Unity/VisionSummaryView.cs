using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class VisionSummaryView : View, IVisionSummaryView
    {
        public event Action? CompleteButtonClicked;

        [Header("Colors")]
        [SerializeField]
        private Color m_HeaderTextColorSuccess;

        [SerializeField]
        private Color m_HeaderTextColorFailure;

        [Header("Stuff")]
        [SerializeField]
        private TextMeshProUGUI m_HeaderText = null!;

        [SerializeField]
        private Interactable m_CompleteButton = null!;

        [SerializeField]
        private GameObject m_Particles = null!;

        [Header("Prefabs")]
        [SerializeField]
        private KeyValueView m_SummaryRowPrefab = null!;

        [SerializeField]
        private Transform m_SummaryRowContainer = null!;

        public void Construct(IEnumerable<KeyValuePair<string, string>> summary)
        {
            m_CompleteButton.PointerClick += OnCompleteButtonClicked;

            foreach (var pair in summary)
            {
                Instantiate(m_SummaryRowPrefab, m_SummaryRowContainer).Construct(pair.Key, pair.Value);
            }
        }

        public void SetSuccess(bool isSuccess)
        {
            Game.Instance.Character.Entity.GetComponent<ActorComponent>().PlayAnimation(isSuccess ? "idle" : "death");

            m_Particles.SetActive(isSuccess);

            if (isSuccess)
            {
                m_HeaderText.color = m_HeaderTextColorSuccess;
                m_HeaderText.text = I18N.Instance.Translate("ui_challenge_completed");
            }
            else
            {
                m_HeaderText.color = m_HeaderTextColorFailure;
                m_HeaderText.text = I18N.Instance.Translate("ui_challenge_failed");
            }
        }

        private void OnCompleteButtonClicked()
        {
            CompleteButtonClicked?.Invoke();
        }
    }
}