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
        private readonly HookEffectData data;

        private Mover targetMover;
        private Mover hookMover;
        private GameObject target;
        private GameObject hook;
        private Lightning beam;

        public HookEffect(HookEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new HookEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            if (target.GetComponent<UnitComponent>().IsMovingViaScript)
            {
                TriggerFinished();
                return;
            }

            this.target = target;

            var actor = Caster.GetComponent<ActorComponent>();
            actor.PlayAnimation("hook_start");

            Timer.Instance.Wait(0.33f, () =>
            {
                var rightHand = actor.Model.GetAttachmentPoint(AttachmentPoint.RightHand);

                this.hook = Object.Instantiate(Resources.Load<GameObject>(this.data.Hook));
                this.hook.transform.position = rightHand.transform.position;

                this.beam = Object.Instantiate(Resources.Load<Lightning>(this.data.Beam));
                this.beam.Initialize(rightHand, this.hook.transform);

                this.hookMover = Mover.Factory(new MoverData(MoverType.Linear, 20, 0, 0, true));
                this.hookMover.Stopped += OnHookMoverStopped;
                this.hookMover.Move(this.hook,
                    this.target.GetComponent<ActorComponent>().Model
                        .GetAttachmentPoint(AttachmentPoint.Chest).transform.position);
            });
        }

        private void OnHookMoverStopped()
        {
            var actor = Caster.GetComponent<ActorComponent>();
            actor.PlayAnimation("hook_end");

            this.hookMover.Stopped -= OnHookMoverStopped;

            var destination = GetNearestFreeCell();

            if (destination == null || this.target.IsImmovable())
            {
                Stop();
                return;
            }

            var unitComponent = this.target.GetComponent<UnitComponent>();
            unitComponent.Flags |= UnitFlags.Airborne;
            unitComponent.Flags |= UnitFlags.MovingViaScript;

            this.targetMover = Mover.Factory(new MoverData(MoverType.Parabolic, 15, 0, 2, false));
            this.targetMover.Stopped += OnTargetMoverStopped;
            this.targetMover.Move(this.target, destination.transform.position);

            this.hookMover = Mover.Factory(new MoverData(MoverType.Parabolic, 15, 0, 2, false));
            this.hookMover.Move(
                this.hook, actor.Model.GetAttachmentPoint(AttachmentPoint.RightHand).transform.position);
        }

        private void OnTargetMoverStopped()
        {
            var unitComponent = this.target.GetComponent<UnitComponent>();
            unitComponent.Flags &= ~UnitFlags.Airborne;
            unitComponent.Flags &= ~UnitFlags.MovingViaScript;

            this.target.transform.position = this.targetMover.DestinationCell.transform.position;
            this.targetMover.Stopped -= OnTargetMoverStopped;

            this.targetMover.DestinationCell.OnEnter(this.target);

            Stop();
        }

        private BoardCell GetNearestFreeCell()
        {
            var direction = (this.target.transform.position - Caster.transform.position).normalized;

            return BoardNavigator.Instance.WithinCircle(Caster.transform.position, 2)
                .Where(c => !c.IsOccupied && c.IsWalkable &&
                    Vector3.Dot(direction, (c.transform.position - Caster.transform.position).normalized) >= 0.4f)
                .OrderBy(c => (c.transform.position - Caster.transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        private void Stop()
        {
            this.hookMover?.Stop();
            this.targetMover?.Stop();

            this.hook.DestroyAsVisualEffect();
            this.beam.Destroy();

            TriggerFinished();
        }
    }
}