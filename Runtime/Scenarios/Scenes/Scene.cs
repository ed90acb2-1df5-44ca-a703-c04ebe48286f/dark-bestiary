using System;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Scenarios.Scenes
{
    public class Scene : MonoBehaviour
    {
        public static Scene Active { get; private set; }

        public UnitGroup Entities { get; } = new();
        public SceneData Data { get; private set; }

        private GameObject m_Weather;

        private void Start()
        {
            foreach (var entity in Entities.Alive())
            {
                entity.GetComponent<ActorComponent>().Model.LookAt(BoardNavigator.Instance.Center().transform.position);
            }
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

            TerminateEntities();

            if (m_Weather != null)
            {
                Destroy(m_Weather.gameObject);
            }

            Destroy(gameObject);
        }

        public void Show()
        {
            Active = this;

            MusicManager.Instance.Play(Data.Environment.Music);

            EnableEntities();

            if (m_Weather != null)
            {
                m_Weather.gameObject.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Active = null;

            DisableEntities();

            if (m_Weather != null)
            {
                m_Weather.gameObject.SetActive(false);
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

            m_Weather = Instantiate(
                Resources.Load<GameObject>(weatherInfo.Prefab), new Vector3(0, 0, 15), Quaternion.identity);
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

        public void MoveToSpawnPointsFormation(int team)
        {
            const int maxUnitsPerColumn = 3;

            var units = Entities.AliveInTeam(team);

            var centerX = (int)Math.Ceiling((BoardNavigator.Instance.Board.Width - 1) / 2.0f);
            var centerY = (int)Math.Ceiling((BoardNavigator.Instance.Board.Height - 1) / 2.0f);

            const int spacingX = 2;
            const int spacingY = 1;

            var defaultOffsetY = -Math.Min(
                Mathf.FloorToInt(units.Count / 2f),
                Mathf.FloorToInt(maxUnitsPerColumn / 2f)
            );

            var offsetX = 1;
            var offsetY = defaultOffsetY;

            var counter = 0;

            foreach (var unit in units.OrderBy(WeaponRangeOrderer).ThenByDescending(unit => unit.GetComponent<UnitComponent>().ChallengeRating))
            {
                if (counter >= maxUnitsPerColumn)
                {
                    counter = 0;
                    offsetX += 1;
                    offsetY = defaultOffsetY;
                }

                BoardCell cell;
                var iterations = 0;
                while (true)
                {
                    var cellX = centerX + (offsetX * spacingX - 1) * (team == 1 ? -1 : 1);
                    var cellY = centerY + offsetY * spacingY;
                    cell = BoardNavigator.Instance.CellAt(cellX, cellY);

                    if (offsetY < BoardNavigator.Instance.Board.Height - 1)
                    {
                        offsetY += 1;
                    }
                    else
                    {
                        offsetX += 1;
                        offsetY = defaultOffsetY;
                    }

                    if (cell is { IsWalkable: true, IsOccupied: false })
                    {
                        break;
                    }

                    iterations += 1;
                    if (iterations >= BoardNavigator.Instance.Board.Cells.Count)
                    {
                        Debug.LogError("Cannot find free cell to spawn unit.");
                        return;
                    }
                }

                counter += 1;
                unit.transform.position = cell.transform.position;
            }
        }

        public void MoveToSpawnPointsRandom(int team)
        {
            var units = Entities.AliveInTeam(team).OrderBy(WeaponRangeOrderer).ToArray();
            var cells = BoardNavigator.Instance.Board.Cells
                .Where(cell =>
                {
                    if (cell.IsOccupied ||
                        cell.IsWalkable == false ||
                        cell.X.InRange(4, BoardNavigator.Instance.Board.Width - 4) == false ||
                        cell.Y.InRange(3, BoardNavigator.Instance.Board.Height - 3) == false)
                    {
                        return false;
                    }

                    if (team == 1)
                    {
                        return cell.X < BoardNavigator.Instance.Board.Width / 2;
                    }

                    return cell.X > BoardNavigator.Instance.Board.Width / 2;
                })
                .Random(units.Length)
                .OrderBy(cell => cell.X * (team == 1 ? -1 : 1));

            var index = 0;

            foreach (var cell in cells)
            {
                units[index].transform.position = cell.transform.position;
                index += 1;
            }
        }

        private static int WeaponRangeOrderer(GameObject unit)
        {
            return unit.GetComponent<SpellbookComponent>()?.FirstWeaponSkill()?.GetMaxRange() ?? 100;
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