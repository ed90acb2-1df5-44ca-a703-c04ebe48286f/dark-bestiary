using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class SelectionManager : Singleton<SelectionManager>
    {
        public event Payload<GameObject> AllySelected;
        public event Payload<GameObject> AllyDeselected;
        public event Payload<GameObject> EnemySelected;
        public event Payload<GameObject> EnemyDeselected;

        public GameObject SelectedAlly { get; private set; }
        public GameObject SelectedEnemy { get; private set; }

        [SerializeField] private SpriteRenderer allyFrame;
        [SerializeField] private SpriteRenderer enemyFrame;

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
            Instance.Select(CharacterManager.Instance.Character.Entity);
        }

        public void Select(GameObject entity)
        {
            if (entity.IsOwnedByPlayer())
            {
                this.SelectedAlly = entity;
                this.allyFrame.gameObject.SetActive(true);
                this.allyFrame.transform.position = this.SelectedAlly.transform.position;
                AllySelected?.Invoke(entity);
            }
            else
            {
                this.SelectedEnemy = entity;
                this.enemyFrame.gameObject.SetActive(true);
                this.enemyFrame.transform.position = this.SelectedEnemy.transform.position;
                this.enemyFrame.color = entity.IsOwnedByNeutral()
                    ? Color.yellow.With(a: 0.25f) : Color.red.With(a: 0.25f);
                EnemySelected?.Invoke(entity);
            }
        }

        public void DeselectAll()
        {
            Deselect(this.SelectedAlly);
            Deselect(this.SelectedEnemy);
        }

        public void Deselect(GameObject entity)
        {
            if (entity == this.SelectedAlly)
            {
                this.allyFrame.gameObject.SetActive(false);
                AllyDeselected?.Invoke(entity);
            }
            else if (entity == this.SelectedEnemy)
            {
                this.enemyFrame.gameObject.SetActive(false);
                EnemyDeselected?.Invoke(entity);
            }
        }

        private void Update()
        {
            if (this.SelectedAlly != null)
            {
                this.allyFrame.transform.position = this.SelectedAlly.transform.position;
            }

            if (this.SelectedEnemy != null)
            {
                this.enemyFrame.transform.position = this.SelectedEnemy.transform.position;
            }
        }
    }
}