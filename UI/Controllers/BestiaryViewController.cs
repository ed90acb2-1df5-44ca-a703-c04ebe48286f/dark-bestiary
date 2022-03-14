using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class BestiaryViewController : ViewController<IBestiaryView>
    {
        private readonly IUnitDataRepository unitDataRepository;
        private readonly IUnitRepository unitRepository;
        private readonly CharacterManager characterManager;

        private UnitComponent selected;
        private int level;

        public BestiaryViewController(IBestiaryView view, IUnitDataRepository unitDataRepository,
            IUnitRepository unitRepository, CharacterManager characterManager) : base(view)
        {
            this.unitDataRepository = unitDataRepository;
            this.unitRepository = unitRepository;
            this.characterManager = characterManager;

            this.level = this.characterManager
                .Character
                .Entity
                .GetComponent<ExperienceComponent>()
                .Experience
                .Level;
        }

        protected override void OnInitialize()
        {
            View.Selected += OnUnitSelected;
            View.LevelChanged += OnLevelChanged;
            View.Construct(FindUnlockedMonsters(), this.level);
        }

        private List<UnitData> FindUnlockedMonsters()
        {
            return this.unitDataRepository
                .Find(u => !u.Flags.HasFlag(UnitFlags.Playable) &&
                           !u.Flags.HasFlag(UnitFlags.Dummy) &&
                           this.characterManager.Character.Data.UnlockedMonsters.Any(unitId => u.Id == unitId))
                .ToList();
        }

        protected override void OnTerminate()
        {
            View.Selected -= OnUnitSelected;
            View.LevelChanged -= OnLevelChanged;

            if (this.selected != null)
            {
                this.selected.gameObject.Terminate();
            }
        }

        private void OnLevelChanged(int level)
        {
            this.level = level;

            if (this.selected == null)
            {
                return;
            }

            this.selected.GetComponent<UnitComponent>().Level = this.level;

            View.RefreshProperties(this.selected);
        }

        private void OnUnitSelected(UnitData unit)
        {
            if (this.selected != null)
            {
                if (this.selected.Id == unit.Id)
                {
                    return;
                }

                this.selected.gameObject.Terminate();
            }

            this.selected = this.unitRepository.Find(unit.Id).GetComponent<UnitComponent>();
            this.selected.GetComponent<UnitComponent>().Level = this.level;
            this.selected.transform.position = new Vector3(-200, 0, 0);

            Timer.Instance.WaitForFixedUpdate(() => View.Display(this.selected));
        }
    }
}