using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class BestiaryViewController : ViewController<IBestiaryView>
    {
        private readonly IUnitDataRepository m_UnitDataRepository;
        private readonly IUnitRepository m_UnitRepository;

        private UnitComponent m_Selected;
        private int m_Level;

        public BestiaryViewController(IBestiaryView view, IUnitDataRepository unitDataRepository, IUnitRepository unitRepository) : base(view)
        {
            m_UnitDataRepository = unitDataRepository;
            m_UnitRepository = unitRepository;

            m_Level = Game.Instance
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
            View.Construct(FindUnlockedMonsters(), m_Level);
        }

        private List<UnitData> FindUnlockedMonsters()
        {
            return m_UnitDataRepository
                .Find(u => !u.Flags.HasFlag(UnitFlags.Playable) &&
                           !u.Flags.HasFlag(UnitFlags.Dummy) &&
                           Game.Instance.Character.Data.UnlockedMonsters.Any(unitId => u.Id == unitId))
                .ToList();
        }

        protected override void OnTerminate()
        {
            View.Selected -= OnUnitSelected;
            View.LevelChanged -= OnLevelChanged;

            if (m_Selected != null)
            {
                m_Selected.gameObject.Terminate();
            }
        }

        private void OnLevelChanged(int level)
        {
            m_Level = level;

            if (m_Selected == null)
            {
                return;
            }

            m_Selected.GetComponent<UnitComponent>().Level = m_Level;

            View.RefreshProperties(m_Selected);
        }

        private void OnUnitSelected(UnitData unit)
        {
            if (m_Selected != null)
            {
                if (m_Selected.Id == unit.Id)
                {
                    return;
                }

                m_Selected.gameObject.Terminate();
            }

            m_Selected = m_UnitRepository.Find(unit.Id).GetComponent<UnitComponent>();
            m_Selected.GetComponent<UnitComponent>().Level = m_Level;
            m_Selected.transform.position = new Vector3(-200, 0, 0);

            Timer.Instance.WaitForFixedUpdate(() => View.Display(m_Selected));
        }
    }
}