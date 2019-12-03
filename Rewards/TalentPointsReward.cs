using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Rewards
{
    public class TalentPointsReward : Reward
    {
        public int Count { get; }

        public TalentPointsReward(TalentPointsRewardData data)
        {
            Count = data.Count;
        }

        protected override void OnPrepare(GameObject entity)
        {
        }

        protected override void OnClaim(GameObject entity)
        {
            entity.GetComponent<TalentsComponent>().Points += Count;
        }
    }
}