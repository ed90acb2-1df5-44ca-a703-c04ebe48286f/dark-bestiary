using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Skills;
using UnityEngine;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.UI.Elements
{
    public class FloatingActionBar : FloatingUi
    {
        private static event Action<bool> AlwaysShowToggled;
        private static event Action SettingsChanged;

        private static bool s_HideSkills;

        [SerializeField] private FloatingActionBarSkill m_SkillPrefab;
        [SerializeField] private Transform m_SkillContainer;

        private static bool s_AlwaysShow;

        private SpellbookComponent m_Spellbook;
        private List<FloatingActionBarSkill> m_Skills;

        public static void HideSkills(bool value)
        {
            s_HideSkills = value;
            SettingsChanged?.Invoke();
        }

        public static void AlwaysShow(bool value)
        {
            s_AlwaysShow = value;
            AlwaysShowToggled?.Invoke(value);
        }

        public void Initialize(SpellbookComponent spellbook)
        {
            AlwaysShowToggled += SetAlwaysShow;
            SettingsChanged += OnSettingsChanged;

            m_Spellbook = spellbook;
            m_Spellbook.Terminated += OnTerminated;
            m_Spellbook.SkillLearned += OnSkillLearned;
            m_Spellbook.SkillUnlearned += OnSkillUnlearned;

            m_Skills = new List<FloatingActionBarSkill>();

            foreach (var slot in m_Spellbook.Slots)
            {
                OnSkillLearned(slot.Skill);
            }

            Initialize(s_AlwaysShow, false, AttachmentPoint.None,
                spellbook.GetComponent<ActorComponent>(),
                spellbook.GetComponent<HealthComponent>()
            );

            OnSettingsChanged();

            MaybeHide();
        }

        private void OnTerminated(Component component)
        {
            AlwaysShowToggled -= SetAlwaysShow;
            SettingsChanged -= OnSettingsChanged;

            m_Spellbook.Terminated -= OnTerminated;
            m_Spellbook.SkillLearned -= OnSkillLearned;
            m_Spellbook.SkillUnlearned -= OnSkillUnlearned;

            foreach (var skill in m_Skills)
            {
                skill.Terminate();
            }

            Terminate();
        }

        private void OnSettingsChanged()
        {
            gameObject.SetActive(!s_HideSkills);
        }

        private void OnSkillLearned(Skill skill)
        {
            if (skill.Type == SkillType.Weapon || skill.IsEmpty())
            {
                return;
            }

            var element = Instantiate(m_SkillPrefab, m_SkillContainer);
            element.Initialize(skill);

            m_Skills.Add(element);
        }

        private void OnSkillUnlearned(Skill skill)
        {
            if (skill.Type == SkillType.Weapon || skill.IsEmpty())
            {
                return;
            }

            var element = m_Skills.FirstOrDefault(s => s.Skill == skill);

            if (element == null)
            {
                return;
            }

            element.Terminate();
            Destroy(element.gameObject);

            m_Skills.Remove(element);
        }
    }
}