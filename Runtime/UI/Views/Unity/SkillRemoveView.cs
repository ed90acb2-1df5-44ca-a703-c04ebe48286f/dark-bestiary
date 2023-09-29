using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class SkillRemoveView : View, ISkillRemoveView
    {
        public event Action<Skill>? RemoveButtonClicked;
        public event Action? CancelButtonClicked;

        [SerializeField]
        private SkillSelectSkillView m_SkillPrefab = null!;

        [SerializeField]
        private Transform m_SkillContainer = null!;

        [SerializeField]
        private Interactable m_CancelButton = null!;

        [SerializeField]
        private Interactable m_RemoveButton = null!;

        private readonly List<SkillSelectSkillView> m_SkillViews = new();

        private SkillSelectSkillView? m_Selected;

        private void Start()
        {
            m_RemoveButton.PointerClick += OnRemoveButtonPointerClick;
            m_CancelButton.PointerClick += OnCancelButtonPointerClick;
        }

        public void Construct(IEnumerable<Skill> skills)
        {
            foreach (var skillView in m_SkillViews)
            {
                skillView.Clicked -= OnSkillViewClicked;
                Destroy(skillView.gameObject);
            }

            m_SkillViews.Clear();

            foreach (var skill in skills)
            {
                var skillView = Instantiate(m_SkillPrefab, m_SkillContainer);
                skillView.Clicked += OnSkillViewClicked;
                skillView.Construct(skill);
                m_SkillViews.Add(skillView);
            }

            OnSkillViewClicked(m_SkillViews.First());
        }

        private void OnSkillViewClicked(SkillSelectSkillView skillView)
        {
            m_Selected?.Deselect();
            m_Selected = skillView;
            m_Selected.Select();
        }

        private void OnRemoveButtonPointerClick()
        {
            if (m_Selected == null)
            {
                return;
            }

            RemoveButtonClicked?.Invoke(m_Selected.Skill);
        }

        private void OnCancelButtonPointerClick()
        {
            CancelButtonClicked?.Invoke();
        }
    }
}