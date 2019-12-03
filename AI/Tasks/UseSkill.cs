using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Exceptions;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.AI.Tasks
{
    public abstract class UseSkill : BehaviourTreeLogicNode
    {
        private readonly int skillId;

        private bool? success;

        protected UseSkill(BehaviourTreePropertiesData properties) : base(properties)
        {
            this.skillId = properties.SkillId;
        }

        protected abstract object GetTarget(BehaviourTreeContext context);

        protected override void OnOpen(BehaviourTreeContext context)
        {
            var skill = context.Entity.GetComponent<SpellbookComponent>().FindOnActionBar(this.skillId);

            this.success = null;

            try
            {
                skill.Used += OnSkillUsed;
                skill.Use(GetTarget(context));
            }
            catch (GameplayException exception)
            {
                this.success = false;
                skill.Used -= OnSkillUsed;
            }
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (this.success == null)
            {
                return BehaviourTreeStatus.Running;
            }

            return this.success == true ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }

        private void OnSkillUsed(SkillUseEventData data)
        {
            this.success = true;
            data.Skill.Used -= OnSkillUsed;
        }
    }
}