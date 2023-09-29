using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Scenarios.Behaviours
{
    public class OnEpisodeCompleteRunToExit : OnEpisodeCompleteRunTo
    {
        protected override Vector3 GetDestination()
        {
            return BoardNavigator.Instance.Right().transform.position;
        }
    }
}