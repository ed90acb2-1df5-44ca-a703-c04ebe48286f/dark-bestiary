using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Exceptions;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.AI.Tasks
{
    public abstract class UseSkill : BehaviourTreeLogicNode
    {
        private readonly int m_SkillId;

        private bool? m_Success;

        protected UseSkill(BehaviourTreePropertiesData properties) : base(properties)
        {
            m_SkillId = properties.SkillId;
        }

        protected abstract object GetTarget(BehaviourTreeContext context);

        protected override void OnOpen(BehaviourTreeContext context)
        {
            var skill = context.Entity.GetComponent<SpellbookComponent>().Get(m_SkillId);

            m_Success = null;

            try
            {
                skill.Used += OnSkillUsed;
                skill.Use(GetTarget(context));
            }
            catch (GameplayException exception)
            {
                m_Success = false;
                skill.Used -= OnSkillUsed;
                Debug.LogError(skill.Name + " " + exception.Message);
            }
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (m_Success == null)
            {
                return BehaviourTreeStatus.Running;
            }

            return m_Success == true ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }

        private void OnSkillUsed(SkillUseEventData data)
        {
            m_Success = true;
            data.Skill.Used -= OnSkillUsed;
        }
    }
}