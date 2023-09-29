using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class UseSkillOnTargetPoint : UseSkill
    {
        public UseSkillOnTargetPoint(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override object GetTarget(BehaviourTreeContext context)
        {
            return context.RequireTargetPoint();
        }
    }
}