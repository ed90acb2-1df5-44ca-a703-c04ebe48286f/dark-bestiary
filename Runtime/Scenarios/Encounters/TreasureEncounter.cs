using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios.Scenes;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.Scenarios.Encounters
{
    public class TreasureEncounter : Encounter
    {
        private const string c_PanelPrefabPath = "Prefabs/UI/TreasureEncounterPanel";
        private const string c_SapperCellPrefabPath = "Prefabs/SapperCell";
        private const string c_ExplosionPrefabPath = "Prefabs/VFX/ExplosionLarge";
        private const string c_TreasureChestPrefabPath = "Prefabs/VFX/TreasureChest/TreasureChest";

        private const int c_ShovelItemId = 703;
        private const int c_ShovelDigSkillId = 285;
        private const int c_ShovelMarkSkillId = 286;

        private readonly IItemRepository m_ItemRepository;
        private readonly GameObject m_Character;
        private readonly int m_TotalBombCount;

        private TreasureEncounterPanel m_Panel;
        private List<SapperCell> m_SapperCells;
        private List<Item> m_Weapons;
        private Item m_Shovel;

        public TreasureEncounter(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
            m_Character = Game.Instance.Character.Entity;
            m_TotalBombCount = Rng.Range(8, 12);
        }

        protected override void OnStart()
        {
            CreateCells();
            CreatePanel();

            m_Shovel = m_ItemRepository.Find(c_ShovelItemId);
            m_Weapons = m_Character.GetComponent<EquipmentComponent>().UnequipWeapon();

            m_Character.GetComponent<InventoryComponent>().Pickup(m_Shovel);
            m_Character.GetComponent<EquipmentComponent>().Equip(m_Shovel);

            Skill.AnySkillUsed += OnAnySkillUsed;
        }

        protected override void OnComplete()
        {
            MaybeCleanup();
        }

        protected override void OnFail()
        {
            MaybeCleanup();
        }

        protected override void OnStop()
        {
            MaybeCleanup();
        }

        private void MaybeCleanup()
        {
            if (IsCompletedOrFailed)
            {
                return;
            }

            Skill.AnySkillUsed -= OnAnySkillUsed;

            m_Panel.CompleteButtonClicked -= OnCompleteButtonClicked;
            Object.Destroy(m_Panel.gameObject);

            var equipment = m_Character.GetComponent<EquipmentComponent>();

            if (equipment.IsEquipped(m_Shovel))
            {
                equipment.Unequip(m_Shovel);
            }

            var inventory = m_Character.GetComponent<InventoryComponent>();

            inventory.Remove(m_Shovel);

            foreach (var weapon in m_Weapons)
            {
                if (weapon.IsEmpty || !inventory.Contains(weapon))
                {
                    continue;
                }

                equipment.Equip(weapon);
            }

            foreach (var sapperCell in m_SapperCells)
            {
                sapperCell.Opened -= OnCellOpened;
                Object.Destroy(sapperCell.gameObject);
            }
        }

        private void CreatePanel()
        {
            m_Panel = Object.Instantiate(
                Resources.Load<TreasureEncounterPanel>(c_PanelPrefabPath),
                UIManager.Instance.ViewCanvas.transform);

            m_Panel.ChangeBombCount(0, m_TotalBombCount);
            m_Panel.CompleteButtonClicked += OnCompleteButtonClicked;
        }

        private void OnCompleteButtonClicked()
        {
            AddExperienceAndLoot();
            Complete();
        }

        private void CreateCells()
        {
            var sapperCellPrefab = Resources.Load<SapperCell>(c_SapperCellPrefabPath);

            m_SapperCells = Board.Instance.Cells.Walkable()
                .Select(cell => Object.Instantiate(sapperCellPrefab, cell.transform.position, Quaternion.identity))
                .ToList();

            foreach (var sapperCell in m_SapperCells.Random(m_TotalBombCount))
            {
                sapperCell.IsBomb = true;
            }

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                foreach (var sapperCell in m_SapperCells)
                {
                    var bombs = BoardNavigator.Instance
                        .WithinSquare(sapperCell.transform.position, 1)
                        .Select(c => c.GameObjectsInside.FirstOrDefault(o => o.GetComponent<SapperCell>()))
                        .NotNull()
                        .Count(c => c.GetComponent<SapperCell>().IsBomb);

                    sapperCell.BombsNearby = bombs;
                    sapperCell.Opened += OnCellOpened;
                    sapperCell.Close();
                }

                m_SapperCells.Where(c => c.BombsNearby == 0).Random().Open();
            });
        }

        private void OnCellOpened(SapperCell opened)
        {
            if (IsCompleted)
            {
                return;
            }

            if (opened.IsBomb)
            {
                Object.Instantiate(Resources.Load<GameObject>(c_ExplosionPrefabPath), opened.transform.position, Quaternion.identity).DestroyAsVisualEffect();
                m_Character.GetComponent<ActorComponent>().PlayAnimation("death");
                Fail();
                return;
            }

            var cells = BoardNavigator.Instance.WithinCircle(opened.transform.position, 1).Walkable();

            foreach (var cell in cells)
            {
                var sapperCell = CellAt(cell.transform.position);

                if (sapperCell.IsBomb)
                {
                    continue;
                }

                if (sapperCell.BombsNearby > 0)
                {
                    sapperCell.OpenSilently();
                    continue;
                }

                sapperCell.Open();

                if (IsCompleted)
                {
                    return;
                }
            }

            if (m_SapperCells.Count(c => !c.IsOpened) > m_TotalBombCount)
            {
                return;
            }

            Object.Instantiate(Resources.Load<GameObject>(c_TreasureChestPrefabPath), opened.transform.position, Quaternion.identity, Scene.Active.transform);

            AddExperienceAndLoot();
            Complete();
        }

        private void OnAnySkillUsed(SkillUseEventData data)
        {
            switch (data.Skill.Id)
            {
                case c_ShovelDigSkillId:
                    Open(data.Target.GetPosition());
                    return;
                case c_ShovelMarkSkillId:
                    Mark(data.Target.GetPosition());
                    break;
            }
        }

        private void Open(Vector3 position)
        {
            var cell = CellAt(position);
            cell.SetMarked(false);
            cell.Open();
        }

        private void AddExperienceAndLoot()
        {
            Scenario.Active.AddExperience(m_Character.GetComponent<ExperienceComponent>().Experience.Level * Rng.Range(10, 30));
            Scenario.Active.AddLoot(m_ItemRepository.Random(Rng.Range(5, 15), item => item.RarityId == Constants.c_ItemRarityIdJunk));
        }

        private void Mark(Vector3 position)
        {
            CellAt(position).ToggleMarker();

            m_Panel.ChangeBombCount(m_SapperCells.Count(c => c.IsMarked), m_TotalBombCount);
        }

        private SapperCell CellAt(Vector3 position)
        {
            var boardCell = BoardNavigator.Instance.WithinCircle(position, 0).First();

            return boardCell
                .GameObjectsInside
                .Select(o => o.GetComponent<SapperCell>())
                .NotNull()
                .First();
        }
    }
}