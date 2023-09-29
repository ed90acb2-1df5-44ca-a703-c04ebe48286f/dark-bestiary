using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Randomization;
using DarkBestiary.UI.Controllers;
using Debug = UnityEngine.Debug;

namespace DarkBestiary.Map
{
    public class MapViewController : ViewController<IMapView>
    {
        public bool IsMapConstructed { get; private set; }

        private readonly IMapEncounterDataRepository m_MapEncounterDataRepository;
        private readonly MapEncounterRunner m_MapEncounterRunner;

        private MapEncounterView? m_RunningEncounterView;
        private MapEncounterView? m_PendingCompletion;

        public MapViewController(IMapView view, IMapEncounterDataRepository mapEncounterDataRepository) : base(view)
        {
            m_MapEncounterDataRepository = mapEncounterDataRepository;
            m_MapEncounterRunner = new MapEncounterRunner(OnEncounterCompleted, OnEncounterFailed);
        }

        protected override void OnTerminate()
        {
            m_MapEncounterRunner.Terminate();
        }

        public void ConstructMap()
        {
            const int width = 15;
            const int height = 15;

            View.AnyEncounterViewClicked += OnAnyEncounterViewClicked;
            View.Construct(width, height);
            View.Show();

            if (Game.Instance.Character.Data.Map == null)
            {
                View.CreateEncounters(GetEncounters(width * height));
                View.RunInitialRoutine();
            }
            else
            {
                try
                {
                    LoadMap(Game.Instance.Character.Data.Map);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    View.CreateEncounters(GetEncounters(width * height));
                    View.RunInitialRoutine();
                }
            }

            IsMapConstructed = true;
        }

        public void Enter()
        {
            MusicManager.Instance.Play("event:/Music/Visions");

            View.Show();
            View.Enter();

            if (m_PendingCompletion != null)
            {
                View.OnEncounterCompleted(m_PendingCompletion);
                m_PendingCompletion = null;
                Save();
            }
        }

        public void Exit()
        {
            View.Hide();
            View.Exit();
        }

        private void OnAnyEncounterViewClicked(MapEncounterView mapEncounterView)
        {
            m_RunningEncounterView = mapEncounterView;
            m_MapEncounterRunner.Run(m_RunningEncounterView.Data);
        }

        private void OnEncounterCompleted()
        {
            if (View.IsVisible)
            {
                View.OnEncounterCompleted(m_RunningEncounterView);
            }
            else
            {
                m_PendingCompletion = m_RunningEncounterView;
            }

            Save();
        }

        private void Save()
        {
            Game.Instance.Character.Data.Map = GatherSaveData();
            Game.Instance.Save();
        }

        private void OnEncounterFailed()
        {
        }

        private List<MapEncounterData> GetEncounters(int count)
        {
            var table = new RandomTable(count);

            foreach (var vision in m_MapEncounterDataRepository.Find(x => x.IsFinal == false).Select(PrepareEncounterData))
            {
                table.AddEntry(new RandomTableMapEncounterEntry(
                    vision,
                    new RandomTableEntryParameters(vision.Probability,
                        vision.IsUnique,
                        vision.IsGuaranteed,
                        vision.IsEnabled
                    )
                ));
            }

            var visions = table.Evaluate().OfType<RandomTableMapEncounterEntry>().Select(x => x.Value).Shuffle().ToList();
            visions[^1] = PrepareEncounterData(m_MapEncounterDataRepository.Find(x => x.IsFinal).Random());

            return visions;
        }

        private MapEncounterData PrepareEncounterData(MapEncounterData encounterData)
        {
            if (encounterData.Type != MapEncounterType.Random)
            {
                return encounterData;
            }

            var randomEncounter = new MapEncounterData(m_MapEncounterDataRepository.FindOrFail(encounterData.Encounters.Random()));
            randomEncounter.Icon = encounterData.Icon;
            randomEncounter.NameKey = encounterData.NameKey;
            randomEncounter.DescriptionKey = encounterData.DescriptionKey;
            randomEncounter.Probability = encounterData.Probability;
            randomEncounter.IsFinal = encounterData.IsFinal;
            randomEncounter.IsUnique = encounterData.IsUnique;
            randomEncounter.IsGuaranteed = encounterData.IsGuaranteed;
            randomEncounter.IsEnabled = encounterData.IsEnabled;
            randomEncounter.RarityId = encounterData.RarityId;
            randomEncounter.Sound = string.IsNullOrEmpty(encounterData.Sound) ? randomEncounter.Sound : encounterData.Sound;

            return PrepareEncounterData(randomEncounter);
        }

        private MapSaveData GatherSaveData()
        {
            var save = new MapSaveData();

            save.LastCompletedEncounterIndex = View.LastCompletedEncounterView?.Index ?? -1;

            foreach (var visionView in View.EncounterViews)
            {
                save.Encounters.Add(new MapEncounterSaveData
                {
                    Data = visionView.Data,
                    State = visionView.State,
                    IsHidden = visionView.IsHidden
                });
            }

            return save;
        }

        private void LoadMap(MapSaveData save)
        {
            View.CreateEncounters(save.Encounters.Select(data => data.Data).ToList());

            for (var i = 0; i < save.Encounters.Count; i++)
            {
                if (save.Encounters[i].IsHidden)
                {
                    View.EncounterViews[i].Hide(false);
                    continue;
                }

                switch (save.Encounters[i].State)
                {
                    case MapEncounterState.Locked:
                        View.EncounterViews[i].Lock();
                        break;
                    case MapEncounterState.Unlocked:
                        View.EncounterViews[i].Unlock();
                        break;
                    case MapEncounterState.Completed:
                        View.EncounterViews[i].Complete();
                        break;
                    case MapEncounterState.Revealed:
                        View.EncounterViews[i].Reveal();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (save.LastCompletedEncounterIndex != -1)
            {
                View.ScrollTo(View.EncounterViews[save.LastCompletedEncounterIndex]);
                View.OnEncounterCompleted(View.EncounterViews[save.LastCompletedEncounterIndex]);
            }
        }
    }
}