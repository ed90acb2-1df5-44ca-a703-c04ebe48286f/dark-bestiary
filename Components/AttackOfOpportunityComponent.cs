using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class AttackOfOpportunityComponent : Component
    {
        private bool isAttackTriggered;
        private BehavioursComponent behaviours;
        private SpellbookComponent spellbook;

        protected override void OnInitialize()
        {
            if (gameObject.IsDummy())
            {
                return;
            }

            this.behaviours = GetComponent<BehavioursComponent>();
            this.spellbook = GetComponent<SpellbookComponent>();

            Skill.AnySkillUsed += OnAnySkillUsed;
            MovementComponent.AnyMovementStarted += OnAnyMovementStarted;
            CombatEncounter.AnyCombatRoundStarted += OnAnyCombatRoundStarted;
        }

        protected override void OnTerminate()
        {
            Skill.AnySkillUsed -= OnAnySkillUsed;
            MovementComponent.AnyMovementStarted -= OnAnyMovementStarted;
            CombatEncounter.AnyCombatRoundStarted -= OnAnyCombatRoundStarted;
        }

        private void OnAnyCombatRoundStarted(CombatEncounter combat)
        {
            this.isAttackTriggered = false;
        }

        private void OnAnySkillUsed(SkillUseEventData data)
        {
            if (data.Skill.Flags.HasFlag(SkillFlags.RangedWeapon) ||
                data.Skill.Flags.HasFlag(SkillFlags.Magic) && data.Skill.GetMaxRange() > 1)
            {
                MaybeTriggerAttack(data.Caster);
            }
        }

        private void OnAnyMovementStarted(MovementComponent movement)
        {
            if (!movement.CanMove())
            {
                return;
            }

            MaybeTriggerAttack(movement.gameObject);
        }

        private void MaybeTriggerAttack(GameObject victim)
        {
            if (gameObject.IsEnemyOfPlayer())
            {
                return;
            }

            if (this.isAttackTriggered || !gameObject.IsAlive() || victim.IsAllyOf(gameObject) || this.behaviours.IsUncontrollable || victim.IsInvisible() || CombatEncounter.Active == null)
            {
                return;
            }

            if ((victim.transform.position - transform.position).sqrMagnitude > Board.Instance.CellSize * Board.Instance.CellSize)
            {
                return;
            }

            var attack = this.spellbook.Slots[0].Skill;

            if (attack.IsDefault() || !attack.Flags.HasFlag(SkillFlags.MeleeWeapon) || attack.IsOnCooldown())
            {
                return;
            }

            this.isAttackTriggered = true;
            FloatingTextManager.Instance.Enqueue(gameObject, I18N.Instance.Translate("ui_attack_of_opportunity"), Color.white);

            attack.FaceTargetAndPlayAnimation(victim, () =>
            {
                var effect = attack.Effect.Clone();
                effect.Skill = attack;
                effect.Apply(gameObject, victim);
            });
        }
    }
}