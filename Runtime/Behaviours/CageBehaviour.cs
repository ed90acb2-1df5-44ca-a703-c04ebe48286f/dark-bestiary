using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class CageBehaviour : Behaviour
    {
        public Vector3 Epicenter { get; private set; }
        public Effect LeaveRadiusEffect { get; }
        public int Radius { get; }

        private readonly BoardNavigator m_BoardNavigator;

        public CageBehaviour(CageBehaviourData data,
            BoardNavigator boardNavigator, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_BoardNavigator = boardNavigator;

            LeaveRadiusEffect = effectRepository.Find(data.LeaveRadiusEffectId);
            Radius = data.Radius;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Epicenter = caster.transform.position;
            BoardCell.AnyEntityEnterCell += OnAnyEntityEnterCell;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            BoardCell.AnyEntityEnterCell -= OnAnyEntityEnterCell;
        }

        private void OnAnyEntityEnterCell(GameObject entity, BoardCell cell)
        {
            if (LeaveRadiusEffect == null)
            {
                return;
            }

            if (m_BoardNavigator.WithinCircle(Epicenter, Radius).Contains(cell))
            {
                return;
            }

            LeaveRadiusEffect.Apply(Caster, Target);
            Remove();
        }
    }
}