using DarkBestiary.Scenarios.Scenes;
using UnityEngine;

namespace DarkBestiary.Scenarios.Behaviours
{
    public class OnEpisodeCompleteRunToExit : OnEpisodeCompleteRunTo
    {
        protected override Vector3 GetDestination()
        {
            return Scene.Active.ExitLocation;
        }
    }
}