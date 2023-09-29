using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Pathfinding;
using UnityEngine;

namespace DarkBestiary.AI.Tasks
{
	public class SetTargetPointEscapeSkill : BehaviourTreeLogicNode
	{
		private readonly BoardNavigator m_BoardNavigator;
		private readonly IPathfinder m_Pathfinder;
		private readonly int m_SkillId;

		public SetTargetPointEscapeSkill(BehaviourTreePropertiesData properties, BoardNavigator boardNavigator, IPathfinder pathfinder) : base(properties)
		{
			m_BoardNavigator = boardNavigator;
			m_Pathfinder = pathfinder;
			m_SkillId = properties.SkillId;
		}

		protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
		{
			var skill = context.Entity.GetComponent<SpellbookComponent>().Get(m_SkillId);
			var cells = m_BoardNavigator.WithinSkillRange(context.Entity.transform.position, context.RequireTargetEntity().transform.position, skill);

			var direction = (context.TargetEntity.transform.position - context.Entity.transform.position).normalized;

			var target = cells
				.Where(cell => cell.IsLineOfSightWalkableAndEmpty(context.Entity.transform.position) &&
				               m_Pathfinder.IsTargetReachable(context.Entity, cell.transform.position) &&
				               m_BoardNavigator.WithinCircle(cell.transform.position, 1).ToEntities().Count(e => e.IsEnemyOf(context.Entity)) == 0 &&
				               Vector3.Dot(direction, (cell.transform.position - context.TargetEntity.transform.position).normalized) <= 0.5f )
				.OrderByDescending(cell => (cell.transform.position - context.Entity.transform.position).sqrMagnitude)
				.FirstOrDefault();

			if (target == null)
			{
				return BehaviourTreeStatus.Failure;
			}

			context.TargetPoint = target.transform.position;

			return BehaviourTreeStatus.Success;
		}
	}
}