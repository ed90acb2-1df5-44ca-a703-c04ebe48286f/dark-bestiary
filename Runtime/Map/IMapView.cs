using System;
using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.UI.Views;

namespace DarkBestiary.Map
{
    public interface IMapView : IView
    {
        event Action<MapEncounterView>? AnyEncounterViewClicked;

        MapEncounterView[] EncounterViews { get; }
        MapEncounterView? LastCompletedEncounterView { get; }

        void Construct(int width, int height);

        void CreateEncounters(List<MapEncounterData> encounters);

        void RunInitialRoutine();

        void OnEncounterCompleted(MapEncounterView encounterView);

        void ScrollTo(MapEncounterView destination, Action? callback = null);

        void Enter();

        void Exit();
    }
}