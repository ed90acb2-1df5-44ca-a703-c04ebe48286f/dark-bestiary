using System.Linq;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using DarkBestiary.Movers;
using UnityEngine;

namespace DarkBestiary
{
    public class Missile : MonoBehaviour
    {
        public event Payload<BoardCell> EnterCell;
        public event Payload<BoardCell> ExitCell;
        public event Payload<Missile> Finished;

        public Mover Mover { get; set; }
        public Effect EnterCellEffect { get; set; }
        public Effect ExitCellEffect { get; set; }
        public Effect FinalEffect { get; set; }
        public Effect CollideWithEntitiesEffect { get; set; }
        public Effect CollideWithEnvironmentEffect { get; set; }
        public string ImpactPrefab { get; set; }
        public float FlyHeight { get; set; }
        public bool IsPiercing { get; set; }
        public GameObject Graphics { get; set; }

        private GameObject caster;
        private object target;

        public void Launch(GameObject caster, object target, Vector3 destination)
        {
            if (Mover.IsMoving)
            {
                return;
            }

            this.caster = caster;
            this.target = target;

            transform.LookAt2D(destination);
            SubscribeEvents();
            UpdateFlyHeight();

            if (IsPiercing)
            {
                Mover.FindEnvironmentCollisionAndMove(gameObject, destination);
            }
            else if (CollideWithEntitiesEffect == null && CollideWithEnvironmentEffect == null)
            {
                Mover.Move(gameObject, destination);
            }
            else
            {
                Mover.FindAnyCollisionAndMove(gameObject, destination);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void Update()
        {
            UpdateFlyHeight();
        }

        private void UpdateFlyHeight()
        {
            if (Mathf.Approximately(FlyHeight, 0) || Graphics == null)
            {
                return;
            }

            var position = transform.position;
            Graphics.transform.position = position.With(y: position.y + FlyHeight);
        }

        private void SubscribeEvents()
        {
            Mover.Stopped += OnMoverStopped;
            BoardCell.AnyEntityEnterCell += OnEntityEnterCell;
            BoardCell.AnyEntityExitCell += OnEntityExitCell;
        }

        private void UnsubscribeEvents()
        {
            Mover.Stopped -= OnMoverStopped;
            BoardCell.AnyEntityEnterCell -= OnEntityEnterCell;
            BoardCell.AnyEntityExitCell -= OnEntityExitCell;
        }

        private void OnMoverStopped()
        {
            var impact = Resources.Load<GameObject>(ImpactPrefab);

            if (impact != null)
            {
                Instantiate(impact, Graphics.transform.position, Quaternion.identity).DestroyAsVisualEffect();
            }

            Graphics.DestroyAsVisualEffect();

            Timer.Instance.Wait(2, () => Destroy(gameObject));

            ApplyEffects();

            Finished?.Invoke(this);
        }

        private void ApplyEffects()
        {
            if (Mover.CollidesWithEntity?.IsEnemyOf(this.caster) == true)
            {
                CollideWithEntitiesEffect?.Clone().Apply(this.caster, Mover.CollidesWithEntity);
            }
            else if (Mover.CollidesWithEnvironment)
            {
                CollideWithEnvironmentEffect?.Clone().Apply(this.caster, transform.position.Snapped());
            }

            FinalEffect?.Clone().Apply(this.caster, this.target is GameObject ? this.target : gameObject.transform.position);
        }

        private void OnEntityEnterCell(GameObject entity, BoardCell cell)
        {
            if (entity == gameObject)
            {
                EnterCellEffect?.Clone().Apply(this.caster, entity);
                EnterCell?.Invoke(cell);
            }
        }

        private void OnEntityExitCell(GameObject entity, BoardCell cell)
        {
            if (entity == gameObject && Mover.IsMoving)
            {
                ExitCellEffect?.Clone().Apply(this.caster, entity);
                ExitCell?.Invoke(cell);
            }
        }
    }
}