using DarkBestiary.Components;
using UnityEngine;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.Extensions
{
    public static class GameObjectExtensions
    {
        public static GameObject GetOwner(this GameObject gameObject)
        {
            var summoned = gameObject.GetComponent<SummonedComponent>();
            return summoned == null ? null : summoned.Master;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            var component = gameObject.GetComponent<T>();

            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        public static void Terminate(this GameObject gameObject)
        {
            foreach (var component in gameObject.GetComponents<Component>())
            {
                component.Terminate();
            }

            Object.Destroy(gameObject);
        }

        public static void DestroyAsVisualEffect(this GameObject gameObject)
        {
            if (gameObject.GetComponent<DestroyImmediately>())
            {
                Object.Destroy(gameObject);
                return;
            }

            var animator = gameObject.GetComponent<Animator>();

            if (animator != null)
            {
                animator.Play("death");
                Timer.Instance.Wait(animator.GetAnimationClipLength("death"), () => Object.Destroy(gameObject));
                return;
            }

            var particles = gameObject.GetComponent<ParticleSystem>();

            if (particles == null)
            {
                Object.Destroy(gameObject);
                return;
            }

            if (particles.main.duration > 10)
            {
                Object.Destroy(gameObject);
                return;
            }

            if (particles.main.loop)
            {
                particles.Stop();
            }

            Timer.Instance.Wait(particles.main.duration, () =>
            {
                Object.Destroy(gameObject);
            });
        }

        public static bool IsUnit(this GameObject gameObject)
        {
            return gameObject != null && gameObject.GetComponent<UnitComponent>() != null;
        }

        public static bool IsImmovable(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsImmovable;
        }

        public static bool IsSummoned(this GameObject gameObject)
        {
            return gameObject != null && gameObject.GetComponent<SummonedComponent>() != null;
        }

        public static bool IsCorpseless(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsCorpseless;
        }

        public static bool IsVisible(this GameObject gameObject)
        {
            var actor = gameObject.GetComponent<ActorComponent>();
            return actor != null && actor.IsVisible;
        }

        public static bool IsDummy(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsDummy;
        }

        public static bool IsStructure(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsStructure;
        }

        public static bool IsPlayable(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsPlayable;
        }

        public static bool IsBoss(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsBoss;
        }

        public static bool IsAirborne(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsAirborne;
        }

        public static bool IsMovingViaScript(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsMovingViaScript;
        }

        public static bool IsBlocksMovement(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsBlocksMovement;
        }

        public static bool IsBlocksVision(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsBlocksVision;
        }

        public static bool IsInvisible(this GameObject gameObject)
        {
            var behaviours = gameObject.GetComponent<BehavioursComponent>();
            return behaviours != null && behaviours.IsInvisible;
        }

        public static bool IsUncontrollable(this GameObject gameObject)
        {
            var behaviours = gameObject.GetComponent<BehavioursComponent>();
            return behaviours != null && behaviours.IsUncontrollable;
        }

        public static bool IsAlive(this GameObject gameObject)
        {
            var health = gameObject.GetComponent<HealthComponent>();
            return health != null && health.IsAlive;
        }

        public static bool InTeam(this GameObject gameObject, int teamId)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.TeamId == teamId;
        }

        public static bool IsOwnedByPlayer(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsPlayer;
        }

        public static bool IsOwnedByNeutral(this GameObject gameObject)
        {
            var unit = gameObject.GetComponent<UnitComponent>();
            return unit != null && unit.IsNeutral;
        }

        public static bool IsCharacter(this GameObject gameObject)
        {
            var character = Game.Instance.Character;

            if (character == null)
            {
                return false;
            }

            return gameObject == character.Entity;
        }

        public static bool IsAllyOf(this GameObject gameObject, GameObject target)
        {
            return !gameObject.IsEnemyOf(target);
        }

        public static bool IsAllyOfPlayer(this GameObject gameObject)
        {
            return !gameObject.IsEnemyOfPlayer();
        }

        public static bool IsEnemyOfPlayer(this GameObject gameObject)
        {
            return gameObject.IsEnemyOf(Game.Instance.Character.Entity);
        }

        public static bool IsEnemyOf(this GameObject gameObject, GameObject target)
        {
            var a = gameObject.GetComponent<UnitComponent>();
            var b = target.GetComponent<UnitComponent>();

            if (a == null || b == null)
            {
                return false;
            }

            return a.IsHostile ? !b.IsHostile : b.IsHostile;
        }
    }
}