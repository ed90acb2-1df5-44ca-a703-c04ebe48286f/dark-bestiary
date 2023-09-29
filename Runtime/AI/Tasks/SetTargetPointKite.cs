using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Tasks
{
	public class SetTargetPointKite : BehaviourTreeLogicNode
	{
		private readonly BoardNavigator m_BoardNavigator;
		private readonly IPathfinder m_Pathfinder;
		private readonly int m_Range;

		public SetTargetPointKite(BehaviourTreePropertiesData properties, BoardNavigator boardNavigator, IPathfinder pathfinder) : base(properties)
		{
			m_BoardNavigator = boardNavigator;
			m_Pathfinder = pathfinder;
			m_Range = properties.Range;
		}

		protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
		{
			var enemy = context.RequireTargetEntity();

			var target = m_BoardNavigator.WithinCircle(context.Entity.transform.position, m_Range)
				.Where(cell => cell.IsWalkable && !cell.IsOccupied)
				.Where(cell => m_Pathfinder.IsTargetReachable(context.Entity, cell.transform.position))
				.Where(cell => m_BoardNavigator.WithinCircle(cell.transform.position, 1)
						.ToEntities()
						.Count(e => e.IsEnemyOf(context.Entity)) == 0
				)
				.OrderByDescending(cell => (cell.transform.position - enemy.transform.position).sqrMagnitude)
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