using DarkBestiary.Map;

namespace DarkBestiary.GameStates
{
    public class MapScreen : Screen
    {
        protected override void OnEnter()
        {
            var mapViewController = Game.Instance.GetController<MapViewController>();

            if (mapViewController.IsMapConstructed == false)
            {
                mapViewController.ConstructMap();
            }

            mapViewController.Enter();
        }

        protected override void OnExit()
        {
            Game.Instance.GetController<MapViewController>().Exit();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}