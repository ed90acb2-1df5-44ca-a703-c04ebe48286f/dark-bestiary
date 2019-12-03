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
        public bool StopOnEnvironmentCollision { get; set; }
        public bool StopOnEntityCollision { get; set; }
        public string ImpactPrefab { get; set; }
        public float FlyHeight { get; set; }
        public GameObject Graphics { get; set; }

        private GameObject caster;
        private object target;
        private Vector3 destination;

        public void Launch(GameObject caster, object target, Vector3 destination)
        {
            if (Mover.IsMoving)
            {
                return;
            }

            SubscribeEvents();

            this.caster = caster;
            this.target = target;
            this.destination = destination;

            transform.LookAt2D(destination);

            UpdateFlyHeight();

            Mover.Start(gameObject, this.destination);
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
            Mover.Finished += OnMoverFinished;

            Mover.CollidingWithEntity += OnCollidingWithEntity;
            Mover.CollidingWithEnvironment += OnCollidingWithEnvironment;

            BoardCell.AnyEntityEnterCell += OnEntityEnterCell;
            BoardCell.AnyEntityExitCell += OnEntityExitCell;
        }

        private void UnsubscribeEvents()
        {
            Mover.Finished -= OnMoverFinished;

            Mover.CollidingWithEntity -= OnCollidingWithEntity;
            Mover.CollidingWithEnvironment -= OnCollidingWithEnvironment;

            BoardCell.AnyEntityEnterCell -= OnEntityEnterCell;
            BoardCell.AnyEntityExitCell -= OnEntityExitCell;
        }

        private void OnMoverFinished()
        {
            FinalEffect?.Clone().Apply(
                this.caster, this.target is GameObject ? this.target : gameObject.transform.position);

            var impact = Resources.Load<GameObject>(ImpactPrefab);

            if (impact != null)
            {
                Instantiate(impact, Graphics.transform.position, Quaternion.identity).DestroyAsVisualEffect();
            }

            Graphics.DestroyAsVisualEffect();

            Timer.Instance.Wait(2, () => Destroy(gameObject));

            Finished?.Invoke(this);
        }

        private void OnCollidingWithEnvironment()
        {
            CollideWithEnvironmentEffect?.Clone().Apply(this.caster, transform.position.Snapped());

            if (StopOnEnvironmentCollision)
            {
                Mover.Stop();
            }
        }

        private void OnCollidingWithEntity(GameObject entity)
        {
            if (entity.IsAllyOf(this.caster))
            {
                return;
            }

            CollideWithEntitiesEffect?.Clone().Apply(this.caster, entity);

            if (StopOnEntityCollision)
            {
                Mover.Stop();
            }
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