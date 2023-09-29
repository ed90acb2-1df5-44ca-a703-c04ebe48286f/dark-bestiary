using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.Scenarios
{
    public class SummaryRecorder
    {
        private int m_Rounds;
        private int m_EncountersCompleted;
        private int m_Legendaries;
        private int m_Skills;
        private int m_MonstersSlain;
        private int m_BossesSlain;
        private float m_DamageDealt;
        private float m_HighestDamageDealt;
        private float m_DamageTaken;
        private float m_HighestDamageTaken;
        private float m_HealingTaken;
        private float m_HighestHealingTaken;

        public void Start(Summary summary = new())
        {
            m_Rounds = summary.Rounds;
            m_EncountersCompleted = summary.EncountersCompleted;
            m_Legendaries = summary.Legendaries;
            m_Skills = summary.Skills;
            m_MonstersSlain = summary.MonstersSlain;
            m_BossesSlain = summary.BossesSlain;
            m_DamageDealt = summary.DamageDealt;
            m_HighestDamageDealt = summary.HighestDamageDealt;
            m_DamageTaken = summary.DamageTaken;
            m_HighestDamageTaken = summary.HighestDamageTaken;
            m_HealingTaken = summary.HealingTaken;
            m_HighestHealingTaken = summary.HighestHealingTaken;

            CombatEncounter.AnyCombatRoundStarted += OnRoundStarted;
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
            HealthComponent.AnyEntityHealed += OnEntityHealed;
            InventoryComponent.AnyItemPicked += OnAnyItemPicked;
            SpellbookComponent.AnySkillLearned += OnAnySkillLearned;
        }

        public void Stop()
        {
            CombatEncounter.AnyCombatRoundStarted -= OnRoundStarted;
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
            HealthComponent.AnyEntityHealed -= OnEntityHealed;
            InventoryComponent.AnyItemPicked -= OnAnyItemPicked;
            SpellbookComponent.AnySkillLearned -= OnAnySkillLearned;
        }

        public Summary GetResult()
        {
            return new Summary(
                m_Rounds,
                m_EncountersCompleted,
                m_Skills,
                m_Legendaries,
                m_MonstersSlain,
                m_BossesSlain,
                m_DamageDealt,
                m_HighestDamageDealt,
                m_DamageTaken,
                m_HighestDamageTaken,
                m_HealingTaken,
                m_HighestHealingTaken
            );
        }

        private void OnAnySkillLearned(Skill skill)
        {
            if (skill.Caster.IsCharacter())
            {
                m_Skills++;
            }
        }

        private void OnAnyItemPicked(ItemPickupEventData data)
        {
            if (data.Item.IsEmpty)
            {
                return;
            }

            if (data.Item.Rarity.Type == RarityType.Legendary &&
                data.Item.Inventory.Owner.IsCharacter() &&
                data.Item.IsEquipment)
            {
                m_Legendaries++;
            }
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            if (data.Target.IsOwnedByPlayer())
            {
                m_HealingTaken += data.Healing;
                m_HighestHealingTaken = Mathf.Max(m_HighestHealingTaken, data.Healing);
            }
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Source.IsAllyOfPlayer())
            {
                m_DamageDealt += data.Damage;
                m_HighestDamageDealt = Mathf.Max(m_HighestDamageDealt, data.Damage);
            }
            else
            {
                m_DamageTaken += data.Damage;
                m_HighestDamageTaken = Mathf.Max(m_HighestDamageTaken, data.Damage);
            }
        }

        private void OnAnyEntityDied(EntityDiedEventData payload)
        {
            if (payload.Victim.IsAllyOfPlayer())
            {
                return;
            }

            if (payload.Victim.IsBoss())
            {
                m_BossesSlain++;
            }
            else
            {
                m_MonstersSlain++;
            }
        }

        private void OnRoundStarted(CombatEncounter combat)
        {
            m_Rounds++;
        }
    }
}