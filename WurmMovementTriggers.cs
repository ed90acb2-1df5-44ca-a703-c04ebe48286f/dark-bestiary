using DarkBestiary.Components;
using UnityEngine;

namespace DarkBestiary
{
    [RequireComponent(typeof(Animator))]
    public class WurmMovementTriggers : MonoBehaviour
    {
        private static readonly int WalkStop = Animator.StringToHash("walk_stop");
        private static readonly int WalkStart = Animator.StringToHash("walk_start");

        private MovementComponent movement;
        private Animator animator;

        private void Start()
        {
            this.animator = GetComponent<Animator>();
            this.movement = GetComponentInParent<MovementComponent>();

            if (this.movement == null)
            {
                return;
            }

            this.movement.Started += OnMovementStarted;
            this.movement.Stopped += OnMovementStopped;
        }

        private void OnMovementStarted(MovementComponent movement)
        {
            this.animator.SetTrigger(WalkStart);
        }

        private void OnMovementStopped(MovementComponent movement)
        {
            this.animator.SetTrigger(WalkStop);
        }
    }
}