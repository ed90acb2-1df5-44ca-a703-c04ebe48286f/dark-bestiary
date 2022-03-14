using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;
using Scene = DarkBestiary.Scenarios.Scenes.Scene;

namespace DarkBestiary.Scenarios.Encounters
{
    public class CombatEncounter : Encounter
    {
        public static event Payload<CombatEncounter> AnyCombatStarted;
        public static event Payload<CombatEncounter> AnyCombatEnded;
        public static event Payload<CombatEncounter> AnyCombatRoundStarted;
        public static event Payload<CombatEncounter> AnyCombatTeamTurnStarted;
        public static event Payload<CombatEncounter> AnyCombatTeamTurnEnded;
        public static event Payload<GameObject> AnyCombatTurnStarted;
        public static event Payload<GameObject> AnyCombatTurnEnded;

        public static CombatEncounter Active { get; private set; }
        public List<GameObject> Queue { get; } = new List<GameObject>();
        public GameObject Acting { get; private set; }

        public int RoundNumber { get; private set; }
        public int ActingTeamId { get; private set; }

        protected override void OnStart()
        {
            Active = this;
            HealthComponent.AnyEntityDied += OnEntityDied;
            BehavioursComponent.AnyBehaviourApplied += OnAnyBehaviourApplied;
            Scenario.AnyScenarioExit += OnAnyScenarioExit;

            AudioManager.Instance.PlayCombatStart();
            AnyCombatStarted?.Invoke(this);

            NextRound();
        }

        protected override void OnComplete()
        {
            AudioManager.Instance.PlayCombatEnd();
            AnyCombatEnded?.Invoke(this);

            Cleanup();
        }

        protected override void OnFail()
        {
            AnyCombatEnded?.Invoke(this);
            Cleanup();
        }

        private void Cleanup()
        {
            Active = null;
            HealthComponent.AnyEntityDied -= OnEntityDied;
            BehavioursComponent.AnyBehaviourApplied -= OnAnyBehaviourApplied;
            Scenario.AnyScenarioExit -= OnAnyScenarioExit;
        }

        public bool IsEntityTurn(GameObject entity)
        {
            return Acting == entity;
        }

        public bool TrySwitchTurn(GameObject entity)
        {
            if (!entity.IsOwnedByPlayer() || !CanSwitchTurns(Acting, entity))
            {
                return false;
            }

            AnyCombatTurnEnded?.Invoke(Acting);

            StartTurn(entity);

            return true;
        }

        private bool CanSwitchTurns(GameObject entity1, GameObject entity2)
        {
            if (entity1 == null || entity2 == null || entity1 == entity2)
            {
                return false;
            }

            if (!entity1.IsOwnedByPlayer() || !entity2.IsOwnedByPlayer())
            {
                return false;
            }

            return IsEntityTurn(entity1) && Queue.Contains(entity2);
        }

        public void NextRound()
        {
            ActingTeamId = ActingTeamId == 1 ? 2 : 1;

            Queue.Clear();

            foreach (var entity in Scene.Active.Entities.AliveInTeam(ActingTeamId))
            {
                if (entity == null)
                {
                    continue;
                }

                Queue.Add(entity);
            }

            if (ActingTeamId == 1)
            {
                RoundNumber++;
                AnyCombatRoundStarted?.Invoke(this);
            }

            AnyCombatTeamTurnStarted?.Invoke(this);

            if (Queue.Count == 0 || Scene.Active.Entities.All().All(x => x.IsAllyOfPlayer()))
            {
                Complete();
                return;
            }

            StartTurn(Queue.First());
        }

        private void StartTurn(GameObject entity)
        {
            var behaviours = entity.GetComponent<BehavioursComponent>();

            if (behaviours.IsUncontrollable || !entity.IsAlive())
            {
                SkillQueue.Clear(entity);

                Acting = entity;
                AnyCombatTurnStarted?.Invoke(Acting);

                EndTurn(Acting);

                return;
            }

            Acting = entity;

            SkillQueue.Run(entity, () =>
            {
                AnyCombatTurnStarted?.Invoke(Acting);
            });
        }

        public void EndTurn(GameObject entity)
        {
            if (!IsEntityTurn(entity))
            {
                return;
            }

            Queue.Remove(entity);
            Acting = null;

            AnyCombatTurnEnded?.Invoke(entity);

            if (Queue.Count == 0)
            {
                AnyCombatTeamTurnEnded?.Invoke(this);
                NextRound();
                return;
            }

            StartTurn(Queue.First());
        }

        private void MaybeEndTurn(GameObject entity)
        {
            if (!Queue.Contains(entity))
            {
                return;
            }

            if (IsEntityTurn(entity))
            {
                EndTurn(entity);
            }
        }

        private void OnAnyScenarioExit(Scenario scenario)
        {
            Cleanup();
        }

        private void OnEntityDied(EntityDiedEventData data)
        {
            if (data.Victim.IsCharacter())
            {
                Fail();
                return;
            }

            MaybeEndTurn(data.Victim);
            Queue.Remove(data.Victim);

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                if (Scene.Active.Entities.Alive(entity => entity.IsEnemyOfPlayer()).Count == 0)
                {
                    Complete();
                }
            });
        }

        private void OnAnyBehaviourApplied(Behaviour behaviour)
        {
            if (!behaviour.Target.GetComponent<BehavioursComponent>().IsUncontrollable)
            {
                return;
            }

            Timer.Instance.WaitForEndOfFrame(() =>
            {
                SkillQueue.Clear(behaviour.Target);
                MaybeEndTurn(behaviour.Target);
            });
        }
    }
}