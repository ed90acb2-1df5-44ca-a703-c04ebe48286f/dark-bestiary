using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class UseSkillOnTargetEntity : UseSkill
    {
        public UseSkillOnTargetEntity(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override object GetTarget(BehaviourTreeContext context)
        {
            return context.RequireTargetEntity();
        }
    }
}