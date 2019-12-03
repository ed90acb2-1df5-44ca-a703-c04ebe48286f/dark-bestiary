using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class UseSkillOnTargetPointCorpse : UseSkill
    {
        public UseSkillOnTargetPointCorpse(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override object GetTarget(BehaviourTreeContext context)
        {
            var targetPoint = context.RequireTargetPoint();

            var cell = BoardNavigator.Instance.WithinCircle(targetPoint, 0).First();
            var corpse = cell.GameObjectsInside.Corpses().FirstOrDefault();

            if (corpse == null)
            {
                throw new InvalidSkillTargetException();
            }

            corpse.Consume();

            return targetPoint;
        }
    }
}