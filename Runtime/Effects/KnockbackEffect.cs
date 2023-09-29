using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Movers;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class KnockbackEffect : Effect
    {
        private readonly KnockbackEffectData m_Data;
        private readonly IEffectRepository m_EffectRepository;
        private readonly BoardNavigator m_BoardNavigator;

        private Mover m_Mover;

        public KnockbackEffect(KnockbackEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository, BoardNavigator boardNavigator) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
            m_BoardNavigator = boardNavigator;
        }

        protected override Effect New()
        {
            return new KnockbackEffect(m_Data, Validators, m_EffectRepository, m_BoardNavigator);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            if (target.IsImmovable() || target.GetComponent<BehavioursComponent>().IsImmobilized || target.GetComponent<UnitComponent>().IsMovingViaScript)
            {
                TriggerFinished();
                return;
            }

            var direction = (target.transform.position - OriginPosition()).normalized;
            var distance = Board.Instance.CellSize * m_Data.Distance;
            var destination = target.transform.position + direction * distance;

            var unitComponent = target.GetComponent<UnitComponent>();
            unitComponent.Flags |= UnitFlags.MovingViaScript;

            m_Mover = Mover.Factory(m_Data.Mover);
            m_Mover.Stopped += OnMoverStopped;
            m_Mover.FindAnyCollisionAndMove(target, destination, distance);
            m_Mover.DestinationCell.IsReserved = true;
        }

        private void OnMoverStopped()
        {
            var unitComponent = m_Mover.Entity.GetComponent<UnitComponent>();
            unitComponent.Flags &= ~UnitFlags.MovingViaScript;

            m_Mover.DestinationCell.OnEnter(m_Mover.Entity);
            m_Mover.DestinationCell.IsReserved = false;
            m_Mover.Entity.transform.position = m_Mover.DestinationCell.transform.position.Snapped();

            if (m_Mover.CollidesWithEntity)
            {
                m_EffectRepository.Find(m_Data.CollideWithEntityEffectId)?.Apply(Caster, Target);
            }
            else if (m_Mover.CollidesWithEnvironment)
            {
                m_EffectRepository.Find(m_Data.CollideWithEnvironmentEffectId)?.Apply(Caster, Target);
            }
            else
            {
                m_EffectRepository.Find(m_Data.FinalEffectId)?.Apply(Caster, Target);
            }

            TriggerFinished();
        }
    }
}