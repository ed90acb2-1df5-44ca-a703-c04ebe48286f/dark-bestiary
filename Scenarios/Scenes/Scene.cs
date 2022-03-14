using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.Scenarios.Scenes
{
    public class Scene : MonoBehaviour
    {
        public static Scene Active { get; private set; }

        public event Payload<GameObject> ExitDoor;
        public event Payload<GameObject> EnterDoor;

        public Vector3 EnterLocation => this.enterDoor == null ? Vector3.zero : this.enterDoor.transform.position;
        public Vector3 ExitLocation => this.exitDoor == null ? Vector3.zero : this.exitDoor.transform.position;

        [SerializeField] private Door enterDoor;
        [SerializeField] private Door exitDoor;

        public UnitGroup Entities { get; } = new UnitGroup();
        public SceneData Data { get; private set; }

        private GameObject weather;

        private void Start()
        {
            if (this.enterDoor != null)
            {
                this.enterDoor.Entered += OnEnterDoorEntered;
            }

            if (this.exitDoor != null)
            {
                this.exitDoor.Entered += OnExitDoorEntered;
            }

            var character = CharacterManager.Instance.Character.Entity.GetComponent<ActorComponent>();

            foreach (var entity in Entities.Alive())
            {
                entity.GetComponent<ActorComponent>().Model.LookAt(character.transform.position);
            }

            character.Model.LookAt(Vector3.zero);
        }

        public void Construct(SceneData data)
        {
            Data = data;
            CreateEnvironment();
        }

        public void Initialize()
        {
            UnitComponent.AnyUnitInitialized += OnComponentInitialized;
            UnitComponent.AnyUnitTerminated += OnComponentTerminated;
        }

        public void Terminate()
        {
            UnitComponent.AnyUnitInitialized -= OnComponentInitialized;
            UnitComponent.AnyUnitTerminated -= OnComponentTerminated;

            EnableDoorCells();
            TerminateEntities();

            if (this.weather != null)
            {
                Destroy(this.weather.gameObject);
            }

            Destroy(gameObject);
        }

        public void Show()
        {
            Active = this;

            MusicManager.Instance.Play(Data.Environment.Music);

            EnableEntities();

            if (this.weather != null)
            {
                this.weather.gameObject.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Active = null;

            EnableDoorCells();
            DisableEntities();

            if (this.weather != null)
            {
                this.weather.gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }

        private void CreateEnvironment()
        {
            if (Data.Environment.Weather.Count == 0)
            {
                return;
            }

            var weatherInfo = Data.Environment.Weather.Random();

            if (string.IsNullOrEmpty(weatherInfo.Prefab))
            {
                return;
            }

            this.weather = Instantiate(
                Resources.Load<GameObject>(weatherInfo.Prefab), new Vector3(0, 0, 15), Quaternion.identity);
        }

        public void EnableDoorCells()
        {
            if (this.enterDoor != null)
            {
                this.enterDoor.EnableCell();
            }

            if (this.exitDoor != null)
            {
                this.exitDoor.EnableCell();
            }
        }

        public void CloseEnterDoor()
        {
            if (this.enterDoor == null)
            {
                return;
            }

            this.enterDoor.Close();
        }

        public void CloseExitDoor()
        {
            if (this.exitDoor == null)
            {
                return;
            }

            this.exitDoor.Close();
        }

        public void OpenEnterDoor()
        {
            if (this.enterDoor == null)
            {
                return;
            }

            this.enterDoor.Open();
        }

        public void OpenExitDoor()
        {
            if (this.exitDoor == null)
            {
                return;
            }

            this.exitDoor.Open();
        }

        public void MoveEntitiesAwayFromExitDoor()
        {
            if (this.exitDoor == null)
            {
                return;
            }

            var position = this.exitDoor.transform.position;

            var nearby = BoardNavigator.Instance.WithinCircle(position, 2);
            var others = BoardNavigator.Instance.Board.Cells
                .Where(c => !nearby.Contains(c) && c.IsWalkable && !c.IsOccupied)
                .OrderBy(c => (c.transform.position - position).sqrMagnitude)
                .Take(nearby.Count)
                .ToList();

            var entities = nearby.ToEntities().ToList();

            for (var i = 0; i < entities.Count; i++)
            {
                entities[i].transform.position = others[i].transform.position;
            }
        }

        private void OnEnterDoorEntered(GameObject entity)
        {
            EnterDoor?.Invoke(entity);
        }

        private void OnExitDoorEntered(GameObject entity)
        {
            ExitDoor?.Invoke(entity);
        }

        private void EnableEntities()
        {
            foreach (var entity in Entities.All())
            {
                var actor = entity.GetComponent<ActorComponent>();
                entity.SetActive(true);
                actor.PlayAnimation("idle");
                actor.Show();
            }
        }

        private void DisableEntities()
        {
            foreach (var entity in Entities.All())
            {
                var actor = entity.GetComponent<ActorComponent>();
                actor.PlayAnimation("idle");
                actor.Hide();
                entity.SetActive(false);
            }
        }

        private void TerminateEntities()
        {
            foreach (var entity in Entities.All())
            {
                if (entity.IsCharacter())
                {
                    continue;
                }

                entity.Terminate();
            }
        }

        public void MoveEnemiesToSpawnPoints()
        {
            MoveToSpawnPoints(
                Entities.AliveInTeam(2),
                GetRandomSpawnPoints(
                    Entities.AliveInTeam(1)
                        .Select(entity => entity.transform.position)
                        .Concat(new[]{EnterLocation.With(z: 0), ExitLocation.With(z: 0)})
                        .ToList()
                )
            );
        }

        public void MoveAlliesToEnter()
        {
            MoveToSpawnPoints(
                Entities.AliveInTeam(1),
                GetSpawnPointsNearby(
                    EnterLocation,
                    Entities.AliveInTeam(2)
                        .Select(entity => entity.transform.position)
                        .Concat(new[]{EnterLocation.With(z: 0), ExitLocation.With(z: 0)})
                        .ToList()
                    )
                );
        }

        public void MoveAlliesToExit()
        {
            MoveToSpawnPoints(
                Entities.AliveInTeam(1),
                GetSpawnPointsNearby(
                    ExitLocation,
                    Entities.AliveInTeam(2)
                        .Select(entity => entity.transform.position)
                        .Concat(new[]{EnterLocation.With(z: 0), ExitLocation.With(z: 0)})
                        .ToList()
                )
            );
        }

        private static void MoveToSpawnPoints(IReadOnlyList<GameObject> entities, IReadOnlyList<BoardCell> cells)
        {
            var reversed = entities.OrderBy(e => e.IsCharacter()).ToList();

            for (var i = 0; i < entities.Count; i++)
            {
                reversed[i].transform.position = cells[i].transform.position.With(z: 0);
            }
        }

        private List<BoardCell> GetRandomSpawnPoints(List<Vector3> excluded)
        {
            return BoardNavigator.Instance
                .Walkable()
                .Where(cell => !cell.IsOccupied && excluded.All(e => (e - cell.transform.position).sqrMagnitude > 2f))
                .Shuffle()
                .ToList();
        }

        private List<BoardCell> GetSpawnPointsNearby(Vector3 point, List<Vector3> excluded)
        {
            return BoardNavigator.Instance
                .Walkable()
                .Where(cell => !cell.IsOccupied && excluded.All(e => (e - cell.transform.position).sqrMagnitude > 2f))
                .OrderBy(cell => (cell.transform.position - point).sqrMagnitude)
                .ToList();
        }

        private void OnComponentInitialized(UnitComponent unit)
        {
            Entities.Add(unit.gameObject);
        }

        private void OnComponentTerminated(UnitComponent unit)
        {
            Entities.Remove(unit.gameObject);
        }
    }
}