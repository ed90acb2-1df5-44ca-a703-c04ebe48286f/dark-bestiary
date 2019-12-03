using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class UseSkillOnSelf : UseSkill
    {
        public UseSkillOnSelf(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override object GetTarget(BehaviourTreeContext context)
        {
            return context.Entity;
        }
    }
}