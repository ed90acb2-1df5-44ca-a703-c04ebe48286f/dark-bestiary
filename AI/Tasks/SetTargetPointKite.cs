using System;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Pathfinding;
using UnityEngine;

namespace DarkBestiary.AI.Tasks
{
	public class SetTargetPointKite : BehaviourTreeLogicNode
	{
		private readonly BoardNavigator boardNavigator;
		private readonly IPathfinder pathfinder;
		private readonly int range;

		public SetTargetPointKite(BehaviourTreePropertiesData properties, BoardNavigator boardNavigator, IPathfinder pathfinder) : base(properties)
		{
			this.boardNavigator = boardNavigator;
			this.pathfinder = pathfinder;
			this.range = properties.Range;
		}

		protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
		{
			var enemy = context.RequireTargetEntity();

			var target = this.boardNavigator.WithinCircle(context.Entity.transform.position, this.range)
				.Where(cell => cell.IsWalkable && !cell.IsOccupied)
				.Where(cell => this.pathfinder.IsTargetReachable(context.Entity, cell.transform.position))
				.Where(cell => this.boardNavigator.WithinCircle(cell.transform.position, 1)
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