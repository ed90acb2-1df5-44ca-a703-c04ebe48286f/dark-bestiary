using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Scenarios.Behaviours
{
    public class OnEpisodeCompleteRunToEnter : OnEpisodeCompleteRunTo
    {
        protected override Vector3 GetDestination()
        {
            return BoardNavigator.Instance.Left().transform.position;
        }
    }
}