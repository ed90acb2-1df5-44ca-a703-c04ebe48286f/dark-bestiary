using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Items;
using DarkBestiary.Movers;
using DarkBestiary.Validators;
using DarkBestiary.Visuals;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class HookEffect : Effect
    {
        private readonly HookEffectData m_Data;

        private Mover m_TargetMover;
        private Mover m_HookMover;
        private GameObject m_Target;
        private GameObject m_Hook;
        private Lightning m_Beam;

        public HookEffect(HookEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new HookEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            if (target.GetComponent<UnitComponent>().IsMovingViaScript)
            {
                TriggerFinished();
                return;
            }

            m_Target = target;

            var actor = Caster.GetComponent<ActorComponent>();
            actor.PlayAnimation("hook_start");

            Timer.Instance.Wait(0.33f, () =>
            {
                var rightHand = actor.Model.GetAttachmentPoint(AttachmentPoint.RightHand);

                m_Hook = Object.Instantiate(Resources.Load<GameObject>(m_Data.Hook));
                m_Hook.transform.position = rightHand.transform.position;

                m_Beam = Object.Instantiate(Resources.Load<Lightning>(m_Data.Beam));
                m_Beam.Initialize(rightHand, m_Hook.transform);

                m_HookMover = Mover.Factory(new MoverData(MoverType.Linear, 20, 0, 0, true));
                m_HookMover.Stopped += OnHookMoverStopped;
                m_HookMover.Move(m_Hook,
                    m_Target.GetComponent<ActorComponent>().Model
                        .GetAttachmentPoint(AttachmentPoint.Chest).transform.position);
            });
        }

        private void OnHookMoverStopped()
        {
            var actor = Caster.GetComponent<ActorComponent>();
            actor.PlayAnimation("hook_end");

            m_HookMover.Stopped -= OnHookMoverStopped;

            var destination = GetNearestFreeCell();

            if (destination == null || m_Target.IsImmovable())
            {
                Stop();
                return;
            }

            var unitComponent = m_Target.GetComponent<UnitComponent>();
            unitComponent.Flags |= UnitFlags.Airborne;
            unitComponent.Flags |= UnitFlags.MovingViaScript;

            m_TargetMover = Mover.Factory(new MoverData(MoverType.Parabolic, 15, 0, 2, false));
            m_TargetMover.Stopped += OnTargetMoverStopped;
            m_TargetMover.Move(m_Target, destination.transform.position);

            m_HookMover = Mover.Factory(new MoverData(MoverType.Parabolic, 15, 0, 2, false));
            m_HookMover.Move(
                m_Hook, actor.Model.GetAttachmentPoint(AttachmentPoint.RightHand).transform.position);
        }

        private void OnTargetMoverStopped()
        {
            var unitComponent = m_Target.GetComponent<UnitComponent>();
            unitComponent.Flags &= ~UnitFlags.Airborne;
            unitComponent.Flags &= ~UnitFlags.MovingViaScript;

            m_Target.transform.position = m_TargetMover.DestinationCell.transform.position;
            m_TargetMover.Stopped -= OnTargetMoverStopped;

            m_TargetMover.DestinationCell.OnEnter(m_Target);

            Stop();
        }

        private BoardCell GetNearestFreeCell()
        {
            var direction = (m_Target.transform.position - Caster.transform.position).normalized;

            return BoardNavigator.Instance.WithinCircle(Caster.transform.position, 2)
                .Where(c => !c.IsOccupied && c.IsWalkable &&
                    Vector3.Dot(direction, (c.transform.position - Caster.transform.position).normalized) >= 0.4f)
                .OrderBy(c => (c.transform.position - Caster.transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        private void Stop()
        {
            m_HookMover?.Stop();
            m_TargetMover?.Stop();

            m_Hook.DestroyAsVisualEffect();
            m_Beam.Destroy();

            TriggerFinished();
        }
    }
}