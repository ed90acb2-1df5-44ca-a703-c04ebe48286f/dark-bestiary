using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.AI.Conditions
{
    public class CanUseSkill : BehaviourTreeLogicNode
    {
        private readonly int m_SkillId;

        public CanUseSkill(BehaviourTreePropertiesData properties) : base(properties)
        {
            m_SkillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var skill = context.Entity.GetComponent<SpellbookComponent>().Get(m_SkillId);

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