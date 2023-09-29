using System;
using System.Collections;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Elements;
using DarkBestiary.Utility;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Managers
{
    public class FloatingTextManager : Singleton<FloatingTextManager>
    {
        [SerializeField] private FloatingText m_FloatingTextPrefab;

        private readonly Vector3 m_Offset = new(0, 40, 0);
        private readonly Dictionary<GameObject, Queue<Action>> m_Queue = new();

        private MonoBehaviourPool<FloatingText> m_Pool;
        private float m_Counter;

        private void Start()
        {
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
            HealthComponent.AnyEntityHealed += OnEntityHealed;
            HealthComponent.AnyEntityInvulnerable += OnAnyEntityInvulnerable;
            BehavioursComponent.AnyBehaviourApplied += OnBehaviourApplied;
            BehavioursComponent.AnyBehaviourImmune += OnAnyBehaviourImmune;

            m_Pool = MonoBehaviourPool<FloatingText>.Factory(
                m_FloatingTextPrefab, UIManager.Instance.GameplayCanvas.transform);

            Episode.AnyEpisodeStopped += OnAnyEpisodeStopped;
        }

        private void OnAnyEpisodeStopped(Episode episode)
        {
            m_Queue.Clear();
            StopAllCoroutines();
        }

        public void Enqueue(GameObject target, string text, Color color)
        {
            if (target == null || SettingsManager.Instance.DisableCombatText)
            {
                return;
            }

            if (!m_Queue.ContainsKey(target))
            {
                m_Queue[target] = new Queue<Action>();
            }

            m_Queue[target].Enqueue(() =>
            {
                m_Pool.Spawn().Initialize(text, color,
                    Camera.main.WorldToScreenPoint(target.transform.position) + m_Offset
                );
            });

            if (m_Queue[target].Count == 1)
            {
                StartCoroutine(DequeueCoroutine(target));
            }
        }

        private IEnumerator DequeueCoroutine(GameObject entity)
        {
            while (m_Queue[entity].Count > 0)
            {
                m_Queue[entity].Peek().Invoke();
                yield return new WaitForSeconds(0.6f);
                m_Queue[entity].Dequeue();
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
            if (behaviour is ShieldBehaviour shieldBehaviour)
            {
                Enqueue(behaviour.Target, $"+{shieldBehaviour.Amount.ToString("F0")}", Color.cyan);
                return;
            }

            var text = "";

            if (behaviour.StatusFlags.HasFlag(StatusFlags.Disarm))
            {
                text = EnumTranslator.Translate(StatusFlags.Disarm);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Swiftness))
            {
                text = EnumTranslator.Translate(StatusFlags.Swiftness);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Immobilization))
            {
                text = EnumTranslator.Translate(StatusFlags.Immobilization);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Invisibility))
            {
                text = EnumTranslator.Translate(StatusFlags.Invisibility);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Invulnerability))
            {
                text = EnumTranslator.Translate(StatusFlags.Invulnerability);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Silence))
            {
                text = EnumTranslator.Translate(StatusFlags.Silence);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Confusion))
            {
                text = EnumTranslator.Translate(StatusFlags.Confusion);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Slow))
            {
                text = EnumTranslator.Translate(StatusFlags.Slow);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Stun))
            {
                text = EnumTranslator.Translate(StatusFlags.Stun);
            }

            else if (behaviour.StatusFlags.HasFlag(StatusFlags.Weakness))
            {
                text = EnumTranslator.Translate(StatusFlags.Weakness);
            }

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            Enqueue(behaviour.Target, text, Color.white);
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            if (!Game.Instance.IsScenario)
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
            if (!Game.Instance.IsScenario)
            {
                return;
            }

            if (data.Damage < 1)
            {
                if (data.Damage.Absorbed > 0)
                {
                    Enqueue(data.Target, I18N.Instance.Get("ui_absorb"), Color.white);
                }
                else if (data.Damage.InfoFlags.HasFlag(DamageInfoFlags.Dodged))
                {
                    Enqueue(data.Target, I18N.Instance.Get("ui_dodge") + "!\n", Color.white);
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

            Enqueue(data.Target, text, Color.white);
        }
    }
}