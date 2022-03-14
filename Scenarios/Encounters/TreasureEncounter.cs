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
        private const string PanelPrefabPath = "Prefabs/UI/TreasureEncounterPanel";
        private const string SapperCellPrefabPath = "Prefabs/SapperCell";
        private const string ExplosionPrefabPath = "Prefabs/VFX/ExplosionLarge";
        private const string TreasureChestPrefabPath = "Prefabs/VFX/TreasureChest/TreasureChest";

        private const int ShovelItemId = 703;
        private const int ShovelDigSkillId = 285;
        private const int ShovelMarkSkillId = 286;

        private readonly IItemRepository itemRepository;
        private readonly GameObject character;
        private readonly int totalBombCount;

        private TreasureEncounterPanel panel;
        private List<SapperCell> sapperCells;
        private List<Item> weapons;
        private Item shovel;

        public TreasureEncounter(CharacterManager characterManager, IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
            this.character = characterManager.Character.Entity;
            this.totalBombCount = RNG.Range(8, 12);
        }

        protected override void OnStart()
        {
            CreateCells();
            CreatePanel();

            this.shovel = this.itemRepository.Find(ShovelItemId);
            this.weapons = this.character.GetComponent<EquipmentComponent>().UnequipWeapon();

            this.character.GetComponent<InventoryComponent>().Pickup(this.shovel);
            this.character.GetComponent<EquipmentComponent>().Equip(this.shovel);

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

            this.panel.CompleteButtonClicked -= OnCompleteButtonClicked;
            Object.Destroy(this.panel.gameObject);

            var equipment = this.character.GetComponent<EquipmentComponent>();

            if (equipment.IsEquipped(this.shovel))
            {
                equipment.Unequip(this.shovel);
            }

            var inventory = this.character.GetComponent<InventoryComponent>();

            inventory.Remove(this.shovel);

            foreach (var weapon in this.weapons)
            {
                if (weapon.IsEmpty || !inventory.Contains(weapon))
                {
                    continue;
                }

                equipment.Equip(weapon);
            }

            foreach (var sapperCell in this.sapperCells)
            {
                sapperCell.Opened -= OnCellOpened;
                Object.Destroy(sapperCell.gameObject);
            }
        }

        private void CreatePanel()
        {
            this.panel = Object.Instantiate(
                Resources.Load<TreasureEncounterPanel>(PanelPrefabPath),
                UIManager.Instance.ViewCanvas.transform);

            this.panel.ChangeBombCount(0, this.totalBombCount);
            this.panel.CompleteButtonClicked += OnCompleteButtonClicked;
        }

        private void OnCompleteButtonClicked()
        {
            AddExperienceAndLoot();
            Complete();
        }

        private void CreateCells()
        {
            var sapperCellPrefab = Resources.Load<SapperCell>(SapperCellPrefabPath);

            this.sapperCells = Board.Instance.Cells.Walkable()
                .Select(cell => Object.Instantiate(sapperCellPrefab, cell.transform.position, Quaternion.identity))
                .ToList();

            foreach (var sapperCell in this.sapperCells.Random(this.totalBombCount))
            {
                sapperCell.IsBomb = true;
            }

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                foreach (var sapperCell in this.sapperCells)
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

                this.sapperCells.Where(c => c.BombsNearby == 0).Random().Open();
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
                Object.Instantiate(Resources.Load<GameObject>(ExplosionPrefabPath), opened.transform.position, Quaternion.identity).DestroyAsVisualEffect();
                this.character.GetComponent<ActorComponent>().PlayAnimation("death");
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

            if (this.sapperCells.Count(c => !c.IsOpened) > this.totalBombCount)
            {
                return;
            }

            Object.Instantiate(Resources.Load<GameObject>(TreasureChestPrefabPath), opened.transform.position, Quaternion.identity, Scene.Active.transform);

            AddExperienceAndLoot();
            Complete();
        }

        private void OnAnySkillUsed(SkillUseEventData data)
        {
            switch (data.Skill.Id)
            {
                case ShovelDigSkillId:
                    Open(data.Target.GetPosition());
                    return;
                case ShovelMarkSkillId:
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
            Scenario.Active.AddExperience(this.character.GetComponent<ExperienceComponent>().Experience.Level * RNG.Range(10, 30));
            Scenario.Active.AddLoot(this.itemRepository.Random(RNG.Range(5, 15), item => item.RarityId == Constants.ItemRarityIdJunk));
        }

        private void Mark(Vector3 position)
        {
            CellAt(position).ToggleMarker();

            this.panel.ChangeBombCount(this.sapperCells.Count(c => c.IsMarked), this.totalBombCount);
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