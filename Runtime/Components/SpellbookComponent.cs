using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class SpellbookComponent : Component
    {
        public static event Action<Skill, SkillSet>? AnySkillSetBonusApplied;
        public static event Action<Skill>? AnySkillLearned;

        public event Action<Skill>? SkillLearned;
        public event Action<Skill>? SkillUnlearned;
        public event Action<Skill>? SkillCooldownStarted;
        public event Action<Skill>? SkillCooldownUpdated;
        public event Action<Skill>? SkillCooldownFinished;

        public Skill? LastUsedSkillThisRound { get; private set; }
        public List<SkillSlot> Slots { get; private set; }

        private BehavioursComponent m_Behaviours;

        public SpellbookComponent Construct()
        {
            Slots = new List<SkillSlot>
            {
                new(0, SkillType.Weapon),
                new(1, SkillType.Weapon),
                new(2, SkillType.Common),
                new(3, SkillType.Common),
                new(4, SkillType.Common),
                new(5, SkillType.Common),
                new(6, SkillType.Common),
                new(7, SkillType.Common),
                new(8, SkillType.Common),
                new(9, SkillType.Common),
            };

            return this;
        }

        protected override void OnInitialize()
        {
            m_Behaviours = GetComponent<BehavioursComponent>();

            CombatEncounter.AnyCombatRoundStarted += OnRoundStarted;
            Skill.AnySkillUsed += OnSkillUsed;
        }

        protected override void OnTerminate()
        {
            CombatEncounter.AnyCombatRoundStarted -= OnRoundStarted;
            Skill.AnySkillUsed -= OnSkillUsed;
        }

        private Skill GetDefaultWeaponSkill()
        {
            var skill = Container.Instance.Resolve<ISkillRepository>().Find(Skill.c_DefaultWeaponSkillId);
            skill.Caster = gameObject;

            return skill;
        }

        public Skill? FirstWeaponSkill()
        {
            return Slots
                .Where(s => s.Skill.Type == SkillType.Weapon)
                .OrderBy(s => s.Index)
                .FirstOrDefault()?
                .Skill;
        }

        public Skill? LastWeaponSkill()
        {
            return Slots
                .Where(s => s.Skill.Type == SkillType.Weapon)
                .OrderByDescending(s => s.Index)
                .FirstOrDefault()?
                .Skill;
        }

        public Skill? Get(int id)
        {
            return Slots.Where(slot => slot.Skill.Id == id)
                .Select(slot => slot.Skill)
                .OrderBy(skill => skill.IsOnCooldown())
                .ThenBy(skill => skill.GetCost(ResourceType.ActionPoint))
                .FirstOrDefault();
        }

        public void Learn(Skill skill)
        {
            var slot = FindSuitableSlot(skill);

            if (slot == null)
            {
                Debug.LogError($"Couldn't find slot for skill {skill.Name}.");
                return;
            }

            Learn(slot, skill);
        }

        public void Learn(SkillSlot slot, Skill skill)
        {
            var previousSlot = Slots.FirstOrDefault(s => s.Skill == skill);

            if (previousSlot != null)
            {
                var temp = previousSlot.Skill;
                previousSlot.ChangeSkill(slot.Skill);
                slot.ChangeSkill(temp);

                return;
            }

            if (slot.SkillType != skill.Type)
            {
                throw new WrongSkillTypeException();
            }

            if (!slot.Skill.IsEmpty())
            {
                Unlearn(slot.Skill);
            }

            if (skill.Rarity?.Type == RarityType.Legendary)
            {
                var otherLegendary = Slots.FirstOrDefault(s => s.Skill.Rarity?.Type == RarityType.Legendary);

                if (otherLegendary != null)
                {
                    Unlearn(otherLegendary.Skill);
                    UiErrorFrame.Instance.ShowMessage(I18N.Instance.Translate("exception_only_one_ultimate"));
                }
            }

            SetupSlot(slot, skill);

            if (skill.Behaviour != null)
            {
                m_Behaviours.ApplyStack(skill.Behaviour, gameObject);
            }

            ApplySkillSetBonuses(skill);

            SkillLearned?.Invoke(skill);
            AnySkillLearned?.Invoke(skill);
        }

        public void Unlearn(int skillId)
        {
            var skill = Get(skillId);

            if (skill == null)
            {
                return;
            }

            Unlearn(skill);
        }

        public void Unlearn(Skill skill)
        {
            if (skill.IsEmpty())
            {
                return;
            }

            foreach (var slot in Slots.Where(s => s.Skill == skill))
            {
                CleanupSlot(slot);

                if (skill.Behaviour != null)
                {
                    m_Behaviours.RemoveStack(skill.Behaviour.Id);
                }

                RemoveSkillSetBonuses(skill);
                ApplySkillSetBonuses(skill);

                SkillUnlearned?.Invoke(slot.Skill);
            }
        }

        private SkillSlot FindSuitableSlotOrFail(Skill skill)
        {
            var suitable = FindSuitableSlot(skill);

            if (suitable == null)
            {
                throw new GameplayException($"No suitable skill slot of type {skill.Type} for skill {skill.Name}");
            }

            return suitable;
        }

        private SkillSlot? FindSuitableSlot(Skill skill)
        {
            return Slots
                .OrderBy(slot => slot.Index)
                .FirstOrDefault(slot => slot.SkillType == skill.Type && (slot.Skill.IsEmpty() || slot.Skill.IsDefault()));
        }

        private void SetupSlot(SkillSlot slot, Skill skill)
        {
            skill.Caster = gameObject;
            skill.CooldownStarted += OnSkillCooldownStarted;
            skill.CooldownUpdated += OnSkillCooldownUpdated;
            skill.CooldownFinished += OnSkillCooldownFinished;
            slot.ChangeSkill(skill);
        }

        private void CleanupSlot(SkillSlot slot)
        {
            slot.Skill.CooldownStarted -= OnSkillCooldownStarted;
            slot.Skill.CooldownUpdated -= OnSkillCooldownUpdated;
            slot.Skill.CooldownFinished -= OnSkillCooldownFinished;
            slot.ChangeSkill(slot.SkillType == SkillType.Weapon ? GetDefaultWeaponSkill() : Skill.s_Empty);
        }

        private void OnSkillUsed(SkillUseEventData data)
        {
            if (data.Caster != gameObject)
            {
                return;
            }

            LastUsedSkillThisRound = data.Skill;
        }

        private void OnRoundStarted(CombatEncounter arg)
        {
            LastUsedSkillThisRound = null;

            foreach (var slot in Slots)
            {
                slot.Skill.ReduceCooldown(1);
            }
        }

        private void OnSkillCooldownStarted(Skill skill)
        {
            SkillCooldownStarted?.Invoke(skill);
        }

        private void OnSkillCooldownUpdated(Skill skill)
        {
            SkillCooldownUpdated?.Invoke(skill);
        }

        private void OnSkillCooldownFinished(Skill skill)
        {
            SkillCooldownFinished?.Invoke(skill);
        }

        public int GetSkillSetPiecesEquipped(SkillSet set)
        {
            return set.SkillIds.Count(IsOnActionBar);

            bool IsOnActionBar(int skillId)
            {
                return Slots.Any(x => x.Skill.Id == skillId);
            }
        }

        private void ApplySkillSetBonuses(Skill skill)
        {
            if (skill.Sets.Count == 0)
            {
                return;
            }

            foreach (var set in skill.Sets)
            {
                var bestAvailable = set.Behaviours
                    .Where(b => b.Key <= GetSkillSetPiecesEquipped(set))
                    .OrderByDescending(b => b.Key)
                    .FirstOrDefault();

                foreach (var bonus in set.Behaviours)
                {
                    if (bestAvailable.Key == bonus.Key)
                    {
                        if (m_Behaviours.IsBehaviourSetApplied(bonus.Value))
                        {
                            continue;
                        }

                        foreach (var behaviour in bonus.Value)
                        {
                            m_Behaviours.ApplyAllStacks(behaviour, gameObject);
                        }

                        AnySkillSetBonusApplied?.Invoke(skill, set);
                        continue;
                    }

                    if (!m_Behaviours.IsBehaviourSetApplied(bonus.Value))
                    {
                        continue;
                    }

                    foreach (var behaviour in bonus.Value)
                    {
                        m_Behaviours.RemoveAllStacks(behaviour.Id);
                    }
                }
            }
        }

        private void RemoveSkillSetBonuses(Skill skill)
        {
            if (skill.Sets.Count == 0)
            {
                return;
            }

            foreach (var set in skill.Sets)
            {
                var equippedPieces = GetSkillSetPiecesEquipped(set);

                foreach (var bonus in set.Behaviours.OrderByDescending(pair => pair.Key))
                {
                    if (equippedPieces >= bonus.Key)
                    {
                        continue;
                    }

                    foreach (var behaviour in bonus.Value)
                    {
                        m_Behaviours.RemoveAllStacks(behaviour.Id);
                    }
                }
            }
        }

        public void ResetCooldowns()
        {
            foreach (var slot in Slots)
            {
                if (slot.Skill.IsEmpty() || slot.Skill.IsDefault())
                {
                    continue;
                }

                slot.Skill.ResetCooldown();
            }
        }

        public void SyncCooldowns(SpellbookComponent spellbook)
        {
            foreach (var slot in Slots)
            {
                slot.Skill.RunCooldown(spellbook.Slots[slot.Index].Skill.RemainingCooldown);
            }
        }
    }
}