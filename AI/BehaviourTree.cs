using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.AI
{
    public class BehaviourTree
    {
        public int Id { get; set; }
        public IBehaviourTreeNodeParent Root { get; }

        public BehaviourTreeContext Context { get; } = new BehaviourTreeContext();

        private float counter;
        private bool enabled;

        public BehaviourTree(IBehaviourTreeNodeParent root)
        {
            Root = root;
        }

        public void Initialize(GameObject entity)
        {
            Context.Entity = entity;
            Context.Entity.GetComponent<HealthComponent>().Died += OnDied;

            this.enabled = Encounter.IsCombat;
            Context.Combat = CombatEncounter.Active;

            CombatEncounter.AnyCombatStarted += OnAnyCombatStarted;
            CombatEncounter.AnyCombatEnded += OnAnyCombatEnded;
            CombatEncounter.AnyCombatRoundStarted += OnAnyCombatRoundStarted;
        }

        public void Terminate()
        {
            if (Context.Entity != null)
            {
                Context.Entity.GetComponent<HealthComponent>().Died -= OnDied;
            }

            this.enabled = false;

            CombatEncounter.AnyCombatStarted -= OnAnyCombatStarted;
            CombatEncounter.AnyCombatEnded -= OnAnyCombatEnded;
            CombatEncounter.AnyCombatRoundStarted -= OnAnyCombatRoundStarted;
        }

        public IBehaviourTreeNode GetRunningNode()
        {
            if (Context.OpenedNodes.Count > 0)
            {
                return Context.OpenedNodes.First();
            }

            return Root;
        }

        public void Tick(float delta)
        {
            if (!this.enabled)
            {
                return;
            }

            this.counter += delta;

            if (this.counter <= 0.25f)
            {
                return;
            }

            if (Context.TargetEntity != null && !Context.TargetEntity.GetComponent<HealthComponent>().IsAlive)
            {
                Context.TargetEntity = null;
            }

            GetRunningNode().Tick(Context, this.counter);

            this.counter = 0;
        }

        private void OnDied(EntityDiedEventData data)
        {
            this.enabled = false;
        }

        private void OnAnyCombatStarted(CombatEncounter combat)
        {
            this.enabled = true;
            Context.Combat = combat;
        }

        private void OnAnyCombatEnded(CombatEncounter combat)
        {
            this.enabled = false;
            Context.Combat = null;
        }

        private void OnAnyCombatRoundStarted(CombatEncounter combat)
        {
            Context.TargetPoint = null;
            Context.TargetEntity = null;
        }
    }
}