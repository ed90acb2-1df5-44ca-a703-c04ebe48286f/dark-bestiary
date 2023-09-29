using System;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class SelectionManager : Singleton<SelectionManager>
    {
        public event Action<GameObject> AllySelected;
        public event Action<GameObject> AllyDeselected;
        public event Action<GameObject> EnemySelected;
        public event Action<GameObject> EnemyDeselected;

        public GameObject SelectedAlly { get; private set; }
        public GameObject SelectedEnemy { get; private set; }

        [SerializeField] private SpriteRenderer m_AllyFrame;
        [SerializeField] private SpriteRenderer m_EnemyFrame;

        private void Start()
        {
            Episode.AnyEpisodeStarted += OnAnyEpisodeStarted;
            CombatEncounter.AnyCombatTurnStarted += OnAnyCombatTurnStarted;
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
        }

        private void OnAnyCombatTurnStarted(GameObject entity)
        {
            if (!entity.IsOwnedByPlayer())
            {
                return;
            }

            Select(entity);
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            Deselect(data.Victim);
        }

        private void OnAnyEpisodeStarted(Episode episode)
        {
            DeselectAll();
            Instance.Select(Game.Instance.Character.Entity);
        }

        public void Select(GameObject entity)
        {
            if (entity.IsOwnedByPlayer())
            {
                SelectedAlly = entity;
                m_AllyFrame.gameObject.SetActive(true);
                m_AllyFrame.transform.position = SelectedAlly.transform.position;
                AllySelected?.Invoke(entity);
            }
            else
            {
                SelectedEnemy = entity;
                m_EnemyFrame.gameObject.SetActive(true);
                m_EnemyFrame.transform.position = SelectedEnemy.transform.position;
                m_EnemyFrame.color = entity.IsOwnedByNeutral()
                    ? Color.yellow.With(a: 0.25f) : Color.red.With(a: 0.25f);
                EnemySelected?.Invoke(entity);
            }
        }

        public void DeselectAll()
        {
            Deselect(SelectedAlly);
            Deselect(SelectedEnemy);
        }

        public void Deselect(GameObject entity)
        {
            if (entity == SelectedAlly)
            {
                m_AllyFrame.gameObject.SetActive(false);
                AllyDeselected?.Invoke(entity);
            }
            else if (entity == SelectedEnemy)
            {
                m_EnemyFrame.gameObject.SetActive(false);
                EnemyDeselected?.Invoke(entity);
            }
        }

        private void Update()
        {
            if (SelectedAlly != null)
            {
                m_AllyFrame.transform.position = SelectedAlly.transform.position;
            }

            if (SelectedEnemy != null)
            {
                m_EnemyFrame.transform.position = SelectedEnemy.transform.position;
            }
        }
    }
}