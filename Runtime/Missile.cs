using System;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Movers;
using UnityEngine;

namespace DarkBestiary
{
    public class Missile : MonoBehaviour
    {
        public event Action<BoardCell> EnterCell;
        public event Action<BoardCell> ExitCell;
        public event Action<Missile> Finished;

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

        private GameObject m_Caster;
        private object m_Target;

        public void Launch(GameObject caster, object target, Vector3 destination)
        {
            if (Mover.IsMoving)
            {
                return;
            }

            m_Caster = caster;
            m_Target = target;

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
            if (Mover.CollidesWithEntity?.IsEnemyOf(m_Caster) == true)
            {
                CollideWithEntitiesEffect?.Clone().Apply(m_Caster, Mover.CollidesWithEntity);
            }
            else if (Mover.CollidesWithEnvironment)
            {
                CollideWithEnvironmentEffect?.Clone().Apply(m_Caster, transform.position.Snapped());
            }

            FinalEffect?.Clone().Apply(m_Caster, m_Target is GameObject ? m_Target : gameObject.transform.position);
        }

        private void OnEntityEnterCell(GameObject entity, BoardCell cell)
        {
            if (entity == gameObject)
            {
                EnterCellEffect?.Clone().Apply(m_Caster, entity);
                EnterCell?.Invoke(cell);
            }
        }

        private void OnEntityExitCell(GameObject entity, BoardCell cell)
        {
            if (entity == gameObject && Mover.IsMoving)
            {
                ExitCellEffect?.Clone().Apply(m_Caster, entity);
                ExitCell?.Invoke(cell);
            }
        }
    }
}