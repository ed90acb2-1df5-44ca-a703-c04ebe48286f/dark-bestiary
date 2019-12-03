using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.AI.Conditions
{
    public class CanUseSkill : BehaviourTreeLogicNode
    {
        private readonly int skillId;

        public CanUseSkill(BehaviourTreePropertiesData properties) : base(properties)
        {
            this.skillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var skill = context.Entity.GetComponent<SpellbookComponent>().FindOnActionBar(this.skillId);

            if (context.Entity.GetComponent<BehavioursComponent>().IsUncontrollable)
            {
                Debug.LogWarning($"{GetType().Name}: entity is uncontrollable but still in combat queue.");
                return BehaviourTreeStatus.Failure;
            }

            return skill.IsDisabled() || skill.IsOnCooldown() || !skill.HasEnoughResources()
                ? BehaviourTreeStatus.Failure
                : BehaviourTreeStatus.Success;
        }
    }
}