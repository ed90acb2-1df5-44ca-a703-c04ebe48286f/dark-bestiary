using DarkBestiary.AI;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class AiComponent : Component
    {
        public BehaviourTree Tree { get; private set; }

        private UnitComponent unit;

        public AiComponent Construct(BehaviourTree tree)
        {
            Tree = tree;
            return this;
        }

        protected override void OnInitialize()
        {
            this.unit = GetComponent<UnitComponent>();
            Tree.Initialize(gameObject);
        }

        protected override void OnTerminate()
        {
            Tree.Terminate();
        }

        private void FixedUpdate()
        {
            if (this.unit.IsPlayer || this.unit.IsMovingViaScript)
            {
                return;
            }

            Tree?.Tick(Time.deltaTime);
        }
    }
}