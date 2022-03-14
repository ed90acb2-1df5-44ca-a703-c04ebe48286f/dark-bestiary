using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using DarkBestiary.Visions;
using UnityEngine;

namespace DarkBestiary.Scenarios
{
    public class SummaryRecorder
    {
        private int rounds;
        private int visionsCompleted;
        private int legendaries;
        private int skills;
        private int monstersSlain;
        private int bossesSlain;
        private float damageDealt;
        private float highestDamageDealt;
        private float damageTaken;
        private float highestDamageTaken;
        private float healingTaken;
        private float highestHealingTaken;

        public void Start(Summary summary = new Summary())
        {
            this.rounds = summary.Rounds;
            this.visionsCompleted = summary.VisionsCompleted;
            this.legendaries = summary.Legendaries;
            this.skills = summary.Skills;
            this.monstersSlain = summary.MonstersSlain;
            this.bossesSlain = summary.BossesSlain;
            this.damageDealt = summary.DamageDealt;
            this.highestDamageDealt = summary.HighestDamageDealt;
            this.damageTaken = summary.DamageTaken;
            this.highestDamageTaken = summary.HighestDamageTaken;
            this.healingTaken = summary.HealingTaken;
            this.highestHealingTaken = summary.HighestHealingTaken;

            VisionRunner.AnyCompleted += OnAnyVisionCompleted;
            CombatEncounter.AnyCombatRoundStarted += OnRoundStarted;
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
            HealthComponent.AnyEntityHealed += OnEntityHealed;
            InventoryComponent.AnyItemPicked += OnAnyItemPicked;
            SpellbookComponent.AnySkillAdded += OnAnySkillAdded;
        }

        public void Stop()
        {
            VisionRunner.AnyCompleted -= OnAnyVisionCompleted;
            CombatEncounter.AnyCombatRoundStarted -= OnRoundStarted;
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
            HealthComponent.AnyEntityHealed -= OnEntityHealed;
            InventoryComponent.AnyItemPicked -= OnAnyItemPicked;
            SpellbookComponent.AnySkillAdded -= OnAnySkillAdded;
        }

        public Summary GetResult()
        {
            return new Summary(
                this.rounds,
                this.visionsCompleted,
                this.skills,
                this.legendaries,
                this.monstersSlain,
                this.bossesSlain,
                this.damageDealt,
                this.highestDamageDealt,
                this.damageTaken,
                this.highestDamageTaken,
                this.healingTaken,
                this.highestHealingTaken
            );
        }

        private void OnAnyVisionCompleted(VisionView vision)
        {
            this.visionsCompleted++;
        }

        private void OnAnySkillAdded(Skill skill)
        {
            if (skill.Caster.IsCharacter())
            {
                this.skills++;
            }
        }

        private void OnAnyItemPicked(ItemPickupEventData payload)
        {
            if (payload.Item.IsEmpty)
            {
                return;
            }

            if (payload.Item.Rarity.Type == RarityType.Legendary &&
                payload.Item.Inventory.Owner.IsCharacter() &&
                payload.Item.IsEquipment)
            {
                this.legendaries++;
            }
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            if (data.Target.IsOwnedByPlayer())
            {
                this.healingTaken += data.Healing;
                this.highestHealingTaken = Mathf.Max(this.highestHealingTaken, data.Healing);
            }
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Attacker.IsAllyOfPlayer())
            {
                this.damageDealt += data.Damage;
                this.highestDamageDealt = Mathf.Max(this.highestDamageDealt, data.Damage);
            }
            else
            {
                this.damageTaken += data.Damage;
                this.highestDamageTaken = Mathf.Max(this.highestDamageTaken, data.Damage);
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
                this.bossesSlain++;
            }
            else
            {
                this.monstersSlain++;
            }
        }

        private void OnRoundStarted(CombatEncounter combat)
        {
            this.rounds++;
        }
    }
}