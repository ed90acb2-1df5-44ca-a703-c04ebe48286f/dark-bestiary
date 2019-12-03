using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using UnityEngine;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.UI.Elements
{
    public class FloatingActionBar : FloatingUi
    {
        private static event Payload<bool> AlwaysShowToggled;
        private static event Payload SettingsChanged;

        private static bool hideSkills;

        [SerializeField] private FloatingActionBarSkill skillPrefab;
        [SerializeField] private Transform skillContainer;

        private static bool alwaysShow;

        private SpellbookComponent spellbook;
        private List<FloatingActionBarSkill> skills;

        public static void HideSkills(bool value)
        {
            hideSkills = value;
            SettingsChanged?.Invoke();
        }

        public static void AlwaysShow(bool value)
        {
            alwaysShow = value;
            AlwaysShowToggled?.Invoke(value);
        }

        public void Initialize(SpellbookComponent spellbook)
        {
            AlwaysShowToggled += SetAlwaysShow;
            SettingsChanged += OnSettingsChanged;

            this.spellbook = spellbook;
            this.spellbook.Terminated += OnTerminated;
            this.spellbook.SkillLearned += OnSkillLearned;
            this.spellbook.SkillUnlearned += OnSkillUnlearned;

            this.skills = new List<FloatingActionBarSkill>();

            foreach (var slot in this.spellbook.Slots)
            {
                OnSkillLearned(slot.Skill);
            }

            Initialize(alwaysShow, false, AttachmentPoint.None,
                spellbook.GetComponent<ActorComponent>(),
                spellbook.GetComponent<HealthComponent>()
            );

            OnSettingsChanged();
        }

        private void OnTerminated(Component component)
        {
            AlwaysShowToggled -= SetAlwaysShow;
            SettingsChanged -= OnSettingsChanged;

            this.spellbook.Terminated -= OnTerminated;
            this.spellbook.SkillLearned -= OnSkillLearned;
            this.spellbook.SkillUnlearned -= OnSkillUnlearned;

            foreach (var skill in this.skills)
            {
                skill.Terminate();
            }

            Terminate();
        }

        private void OnSettingsChanged()
        {
            gameObject.SetActive(!hideSkills);
        }

        private void OnSkillLearned(Skill skill)
        {
            if (skill.Type == SkillType.Weapon || skill.IsEmpty())
            {
                return;
            }

            var element = Instantiate(this.skillPrefab, this.skillContainer);
            element.Initialize(skill);

            this.skills.Add(element);
        }

        private void OnSkillUnlearned(Skill skill)
        {
            if (skill.Type == SkillType.Weapon || skill.IsEmpty())
            {
                return;
            }

            var element = this.skills.FirstOrDefault(s => s.Skill == skill);

            if (element == null)
            {
                return;
            }

            element.Terminate();
            Destroy(element.gameObject);

            this.skills.Remove(element);
        }
    }
}