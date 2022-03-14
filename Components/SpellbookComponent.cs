using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class SpellbookComponent : Component
    {
        public static event Payload<Skill, SkillSet> AnySkillSetBonusApplied;
        public static event Payload<Skill> AnySkillAdded;

        public event Payload<Skill> SkillLearned;
        public event Payload<Skill> SkillUnlearned;
        public event Payload<Skill> SkillCooldownStarted;
        public event Payload<Skill> SkillCooldownUpdated;
        public event Payload<Skill> SkillCooldownFinished;
        public event Payload<Skill> SkillAdded;
        public event Payload<Skill> SkillRemoved;

        public Skill LastUsedSkill { get; private set; }
        public Skill LastUsedSkillThisRound { get; private set; }
        public List<Skill> Skills { get; private set; }
        public List<SkillSlot> Slots { get; private set; }

        private BehavioursComponent behaviours;

        public SpellbookComponent Construct()
        {
            return Construct(new List<Skill>());
        }

        public SpellbookComponent Construct(List<Skill> skills)
        {
            Skills = skills;

            Slots = new List<SkillSlot>
            {
                new SkillSlot(0, SkillType.Weapon),
                new SkillSlot(1, SkillType.Weapon),
                new SkillSlot(2, SkillType.Common),
                new SkillSlot(3, SkillType.Common),
                new SkillSlot(4, SkillType.Common),
                new SkillSlot(5, SkillType.Common),
                new SkillSlot(6, SkillType.Common),
                new SkillSlot(7, SkillType.Common),
                new SkillSlot(8, SkillType.Common),
                new SkillSlot(9, SkillType.Common),
            };

            return this;
        }

        protected override void OnInitialize()
        {
            this.behaviours = GetComponent<BehavioursComponent>();

            if (Slots[0].Skill.IsEmpty())
            {
                PlaceOnActionBar(Slots[0], gameObject.IsCharacter() || CharacterManager.Instance.Character == null
                    ? GetDefaultWeaponSkill() : Skill.Empty);
            }

            if (Slots[1].Skill.IsEmpty())
            {
                PlaceOnActionBar(Slots[1], gameObject.IsCharacter() || CharacterManager.Instance.Character == null
                    ? GetDefaultWeaponSkill() : Skill.Empty);
            }

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
            var skill = Container.Instance.Resolve<ISkillRepository>().Find(Skill.DefaultWeaponSkillId);
            skill.Caster = gameObject;

            return skill;
        }

        private static void Swap(SkillSlot slotA, SkillSlot slotB)
        {
            var temp = slotA.Skill;
            slotA.ChangeSkill(slotB.Skill);
            slotB.ChangeSkill(temp);
        }

        public Skill FirstWeaponSkill()
        {
            return Slots
                .Where(s => s.Skill.Type == SkillType.Weapon)
                .OrderBy(s => s.Index)
                .FirstOrDefault()?
                .Skill;
        }

        public Skill LastWeaponSkill()
        {
            return Slots
                .Where(s => s.Skill.Type == SkillType.Weapon)
                .OrderByDescending(s => s.Index)
                .FirstOrDefault()?
                .Skill;
        }

        public Skill FindOnActionBar(int id)
        {
            return Slots.Where(slot => slot.Skill.Id == id)
                .Select(slot => slot.Skill)
                .OrderBy(skill => skill.IsOnCooldown())
                .ThenBy(skill => skill.GetCost(ResourceType.ActionPoint))
                .FirstOrDefault();
        }

        public Skill Find(int id)
        {
            return Skills.FirstOrDefault(skill => skill.Id == id);
        }

        public bool IsKnown(int skillId)
        {
            return Skills.Any(s => s.Id == skillId);
        }

        public bool IsOnActionBar(int skillId)
        {
            return Slots.Any(slot => slot.Skill.Id == skillId);
        }

        public bool IsOnActionBarHash(Skill skill)
        {
            return Slots.Any(slot => slot.Skill == skill);
        }

        public void Add(Skill skill, bool placeOnActionBar = true)
        {
            skill.Caster = gameObject;
            Skills.Add(skill);

            if (placeOnActionBar && FindSuitableSlot(skill) != null)
            {
                PlaceOnActionBar(skill);
            }

            SkillAdded?.Invoke(skill);
            AnySkillAdded?.Invoke(skill);
        }

        public void Remove(Skill skill)
        {
            Remove(skill.Id);
        }

        public void Remove(int skillId)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);

            if (skill == null)
            {
                return;
            }

            if (IsOnActionBar(skill.Id))
            {
                RemoveFromActionBar(skill.Id);
            }

            Skills.Remove(skill);
            SkillRemoved?.Invoke(skill);
        }

        public void Add(IEnumerable<Skill> skills)
        {
            foreach (var skill in skills)
            {
                Add(skill, false);
            }
        }

        public void PlaceOnActionBar(Skill skill)
        {
            PlaceOnActionBar(FindSuitableSlotOrFail(skill), skill);
        }

        public void PlaceOnActionBar(SkillSlot slot, Skill skill)
        {
            var previousSlot = Slots.FirstOrDefault(s => s.Skill == skill);

            if (previousSlot != null)
            {
                Swap(slot, previousSlot);
                return;
            }

            if (slot.SkillType != skill.Type)
            {
                throw new WrongSkillTypeException();
            }

            if (!slot.Skill.IsEmpty())
            {
                RemoveFromActionBar(slot.Skill);
            }

            if (skill.Rarity?.Type == RarityType.Legendary)
            {
                var otherLegendary = Slots.FirstOrDefault(s => s.Skill.Rarity?.Type == RarityType.Legendary);

                if (otherLegendary != null)
                {
                    RemoveFromActionBar(otherLegendary.Skill);
                    UiErrorFrame.Instance.ShowMessage(I18N.Instance.Translate("exception_only_one_ultimate"));
                }
            }

            SetupSlot(slot, skill);

            if (skill.Behaviour != null)
            {
                this.behaviours.ApplyStack(skill.Behaviour, gameObject);
            }

            ApplySkillSetBonuses(skill);

            SkillLearned?.Invoke(skill);
        }

        public void RemoveFromActionBar(int skillId)
        {
            foreach (var slot in Slots.Where(s => s.Skill.Id == skillId))
            {
                RemoveFromActionBar(slot.Skill);
            }
        }

        public void RemoveFromActionBar(Skill skill, bool withAlternative = true)
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
                    this.behaviours.RemoveStack(skill.Behaviour.Id);
                }

                RemoveSkillSetBonuses(skill);
                ApplySkillSetBonuses(skill);

                SkillUnlearned?.Invoke(slot.Skill);
            }

            if (withAlternative)
            {
                foreach (var alternative in skill.GetSkills())
                {
                    RemoveFromActionBar(alternative, false);
                }
            }
        }

        public void Replace(Skill skillA, Skill skillB)
        {
            if (skillA.IsOnCooldown())
            {
                throw new SkillIsOnCooldownException();
            }

            var slot = Slots.FirstOrDefault(s => s.Skill == skillA);

            if (slot == null)
            {
                throw new Exception($"Skill {skillA.Name} is not on action bar.");
            }

            if (skillA.EquipmentSlot != null)
            {
                if (skillA.EquipmentSlot.Item.WeaponSkillA == skillA)
                {
                    skillA.EquipmentSlot.Item.WeaponSkillA = skillB;
                }

                if (skillA.EquipmentSlot.Item.WeaponSkillB == skillA)
                {
                    skillA.EquipmentSlot.Item.WeaponSkillB = skillB;
                }
            }

            Remove(skillA);
            RemoveFromActionBar(skillA);

            Add(skillB);
            PlaceOnActionBar(slot, skillB);
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

        private SkillSlot FindSuitableSlot(Skill skill)
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
            slot.ChangeSkill(slot.SkillType == SkillType.Weapon ? GetDefaultWeaponSkill() : Skill.Empty);
        }

        private void OnSkillUsed(SkillUseEventData data)
        {
            if (data.Caster != gameObject)
            {
                return;
            }

            LastUsedSkill = data.Skill;
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
        }

        public int GetSkillSetPiecesObtained(SkillSet set)
        {
            return set.SkillIds.Count(IsKnown);
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
                        if (this.behaviours.IsBehaviourSetApplied(bonus.Value))
                        {
                            continue;
                        }

                        foreach (var behaviour in bonus.Value)
                        {
                            this.behaviours.ApplyAllStacks(behaviour, gameObject);
                        }

                        AnySkillSetBonusApplied?.Invoke(skill, set);
                        continue;
                    }

                    if (!this.behaviours.IsBehaviourSetApplied(bonus.Value))
                    {
                        continue;
                    }

                    foreach (var behaviour in bonus.Value)
                    {
                        this.behaviours.RemoveAllStacks(behaviour.Id);
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
                        this.behaviours.RemoveAllStacks(behaviour.Id);
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