using DarkBestiary.AI;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class AiComponent : Component
    {
        public BehaviourTree Tree { get; private set; }

        private UnitComponent m_Unit;

        public AiComponent Construct(BehaviourTree tree)
        {
            Tree = tree;
            return this;
        }

        protected override void OnInitialize()
        {
            m_Unit = GetComponent<UnitComponent>();
            Tree.Initialize(gameObject);
        }

        protected override void OnTerminate()
        {
            Tree.Terminate();
        }

        private void FixedUpdate()
        {
            if (m_Unit.IsPlayer || m_Unit.IsMovingViaScript)
            {
                return;
            }

            Tree?.Tick(Time.deltaTime);
        }
    }
}