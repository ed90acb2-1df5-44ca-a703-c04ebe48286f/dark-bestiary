using DarkBestiary.Components;
using UnityEngine;

namespace DarkBestiary
{
    [RequireComponent(typeof(Animator))]
    public class WurmMovementTriggers : MonoBehaviour
    {
        private static readonly int s_WalkStop = Animator.StringToHash("walk_stop");
        private static readonly int s_WalkStart = Animator.StringToHash("walk_start");

        private MovementComponent m_Movement;
        private Animator m_Animator;

        private void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Movement = GetComponentInParent<MovementComponent>();

            if (m_Movement == null)
            {
                return;
            }

            m_Movement.Started += OnMovementStarted;
            m_Movement.Stopped += OnMovementStopped;
        }

        private void OnMovementStarted(MovementComponent movement)
        {
            m_Animator.SetTrigger(s_WalkStart);
        }

        private void OnMovementStopped(MovementComponent movement)
        {
            m_Animator.SetTrigger(s_WalkStop);
        }
    }
}