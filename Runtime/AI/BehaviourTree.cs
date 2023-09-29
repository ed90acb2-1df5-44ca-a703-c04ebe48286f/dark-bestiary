using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.AI
{
    public class BehaviourTree
    {
        public int Id { get; set; }
        public IBehaviourTreeNodeParent Root { get; }

        public BehaviourTreeContext Context { get; } = new();

        private float m_Counter;
        private bool m_Enabled;

        public BehaviourTree(IBehaviourTreeNodeParent root)
        {
            Root = root;
        }

        public void Initialize(GameObject entity)
        {
            Context.Entity = entity;
            Context.Entity.GetComponent<HealthComponent>().Died += OnDied;

            m_Enabled = Encounter.IsCombat;
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

            m_Enabled = false;

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
            if (!m_Enabled)
            {
                return;
            }

            m_Counter += delta;

            if (m_Counter <= 0.25f)
            {
                return;
            }

            if (Context.TargetEntity != null && !Context.TargetEntity.GetComponent<HealthComponent>().IsAlive)
            {
                Context.TargetEntity = null;
            }

            GetRunningNode().Tick(Context, m_Counter);

            m_Counter = 0;
        }

        private void OnDied(EntityDiedEventData data)
        {
            m_Enabled = false;
        }

        private void OnAnyCombatStarted(CombatEncounter combat)
        {
            m_Enabled = true;
            Context.Combat = combat;
        }

        private void OnAnyCombatEnded(CombatEncounter combat)
        {
            m_Enabled = false;
            Context.Combat = null;
        }

        private void OnAnyCombatRoundStarted(CombatEncounter combat)
        {
            Context.TargetPoint = null;
            Context.TargetEntity = null;
        }
    }
}