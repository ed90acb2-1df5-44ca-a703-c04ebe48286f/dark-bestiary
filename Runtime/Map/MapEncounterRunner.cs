using System;
using DarkBestiary.Data;
using DarkBestiary.Map.Encounters;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.Map
{
    public class MapEncounterRunner
    {
        private readonly Action m_OnEncounterCompleted;
        private readonly Action m_OnEncounterFailed;

        public readonly VendorViewController VendorViewController;
        public readonly SkillRemoveViewController SkillRemoveViewController;
        public readonly SkillSelectViewController SkillSelectViewController;
        public readonly EquipmentViewController EquipmentViewController;

        private IMapEncounter? m_ActiveEncounter;

        public MapEncounterRunner(Action onEncounterCompleted, Action onEncounterFailed)
        {
            m_OnEncounterCompleted = onEncounterCompleted;
            m_OnEncounterFailed = onEncounterFailed;

            VendorViewController = Container.Instance.Instantiate<VendorViewController>();
            VendorViewController.Initialize();

            SkillRemoveViewController = Game.Instance.GetController<SkillRemoveViewController>();
            SkillSelectViewController = Game.Instance.GetController<SkillSelectViewController>();

            EquipmentViewController = Game.Instance.GetController<EquipmentViewController>();
            EquipmentViewController.View.Connect(VendorViewController.View);
        }

        public void Terminate()
        {
            CleanupActiveEncounter();

            VendorViewController.Terminate();

            EquipmentViewController.View.DisconnectAll();
        }

        public void Run(MapEncounterData data)
        {
            OnEncounterCompleted();
            return;

            m_ActiveEncounter = CreateEncounter(data);
            m_ActiveEncounter.Run(OnEncounterCompleted, OnEncounterFailed);
        }

        private void OnEncounterCompleted()
        {
            m_OnEncounterCompleted.Invoke();

            CleanupActiveEncounter();
        }

        private void OnEncounterFailed()
        {
            m_OnEncounterFailed.Invoke();

            CleanupActiveEncounter();
        }

        private void CleanupActiveEncounter()
        {
            if (m_ActiveEncounter == null)
            {
                return;
            }

            m_ActiveEncounter.Cleanup();
            m_ActiveEncounter = null;
        }

        private IMapEncounter CreateEncounter(MapEncounterData data)
        {
            switch (data.Type)
            {
                case MapEncounterType.Skill:
                    return new SkillSelectMapEncounter(this);
                case MapEncounterType.Vendor:
                    return new VendorMapEncounter(data, this);
                case MapEncounterType.Scenario:
                    return new ScenarioMapEncounter(data);
                case MapEncounterType.Eatery:
                    return new FoodMapEncounter();
                case MapEncounterType.Buff:
                    return new BuffMapEncounter();
                case MapEncounterType.Loot:
                    return new LootMapEncounter(data);
                default:
                    throw new ArgumentOutOfRangeException($"Unknown map encounter type: {data.Type}");
            }
        }
    }
}