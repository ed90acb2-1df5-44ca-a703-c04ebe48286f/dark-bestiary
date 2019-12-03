using System;
using System.Collections;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Elements;
using DarkBestiary.Utility;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Managers
{
    public class FloatingTextManager : Singleton<FloatingTextManager>
    {
        [SerializeField] private FloatingText floatingTextPrefab;

        private readonly Vector3 offset = new Vector3(0, 40, 0);
        private readonly Dictionary<GameObject, Queue<Action>> queue = new Dictionary<GameObject, Queue<Action>>();

        private MonoBehaviourPool<FloatingText> pool;
        private float counter;

        private void Start()
        {
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
            HealthComponent.AnyEntityHealed += OnEntityHealed;
            HealthComponent.AnyEntityInvulnerable += OnAnyEntityInvulnerable;
            BehavioursComponent.AnyBehaviourApplied += OnBehaviourApplied;
            BehavioursComponent.AnyBehaviourImmune += OnAnyBehaviourImmune;

            this.pool = MonoBehaviourPool<FloatingText>.Factory(
                this.floatingTextPrefab, UIManager.Instance.GameplayCanvas.transform);

            Episode.AnyEpisodeStopped += OnAnyEpisodeStopped;
        }

        private void OnAnyEpisodeStopped(Episode episode)
        {
            this.queue.Clear();
            StopAllCoroutines();
        }

        public void Enqueue(GameObject target, string text, Color color)
        {
            if (target == null || SettingsManager.Instance.DisableCombatText)
            {
                return;
            }

            if (!this.queue.ContainsKey(target))
            {
                this.queue[target] = new Queue<Action>();
            }

            this.queue[target].Enqueue(() =>
            {
                this.pool.Spawn().Initialize(text, color,
                    Camera.main.WorldToScreenPoint(target.transform.position) + this.offset
                );
            });

            if (this.queue[target].Count == 1)
            {
                StartCoroutine(DequeueCoroutine(target));
            }
        }

        private IEnumerator DequeueCoroutine(GameObject entity)
        {
            while (this.queue[entity].Count > 0)
            {
                this.queue[entity].Peek().Invoke();
                yield return new WaitForSeconds(0.6f);
                this.queue[entity].Dequeue();
            }
        }

        private void OnAnyBehaviourImmune(GameObject entity, Behaviour behaviour)
        {
            Enqueue(entity, I18N.Instance.Get("ui_immune"), Color.white);
        }

        private void OnAnyEntityInvulnerable(GameObject entity)
        {
            Enqueue(entity, I18N.Instance.Get("ui_invulnerable"), Color.white);
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            var text = new I18NString();

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Disarm))
            {
                text = EnumTranslator.Translate(StatusFlags.Disarm);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Swiftness))
            {
                text = EnumTranslator.Translate(StatusFlags.Swiftness);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Immobilization))
            {
                text = EnumTranslator.Translate(StatusFlags.Immobilization);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Invisibility))
            {
                text = EnumTranslator.Translate(StatusFlags.Invisibility);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Invulnerability))
            {
                text = EnumTranslator.Translate(StatusFlags.Invulnerability);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Silence))
            {
                text = EnumTranslator.Translate(StatusFlags.Silence);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Confusion))
            {
                text = EnumTranslator.Translate(StatusFlags.Confusion);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Slow))
            {
                text = EnumTranslator.Translate(StatusFlags.Slow);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Stun))
            {
                text = EnumTranslator.Translate(StatusFlags.Stun);
            }

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Weakness))
            {
                text = EnumTranslator.Translate(StatusFlags.Weakness);
            }

            if (text.IsNullOrEmpty())
            {
                return;
            }

            Enqueue(behaviour.Target, text, Color.white);
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            if (!Game.Instance.State.IsScenario)
            {
                return;
            }

            if (data.Healing < 1)
            {
                return;
            }

            Enqueue(data.Target, $"+{(int) data.Healing}", Color.green);
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            if (!Game.Instance.State.IsScenario)
            {
                return;
            }

            if (data.Damage < 1)
            {
                if (data.Damage.Absorbed > 0)
                {
                    Enqueue(data.Victim, I18N.Instance.Get("ui_absorb"), Color.white);
                }
                else if (data.Damage.InfoFlags.HasFlag(DamageInfoFlags.Dodged))
                {
                    Enqueue(data.Victim, I18N.Instance.Get("ui_dodge") + "!\n", Color.white);
                }

                return;
            }

            var text = "";

            if (data.Damage.InfoFlags.HasFlag(DamageInfoFlags.Critical))
            {
                text = I18N.Instance.Get("ui_critical") + "!\n";
            }

            if (data.Damage.InfoFlags.HasFlag(DamageInfoFlags.Blocked))
            {
                text = I18N.Instance.Get("ui_block") + "!\n";
            }

            text += (int) data.Damage;

            Enqueue(data.Victim, text, Color.white);
        }
    }
}