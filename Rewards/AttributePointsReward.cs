using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Rewards
{
    public class AttributePointsReward : Reward
    {
        public int Count { get; }

        public AttributePointsReward(AttributePointsRewardData data)
        {
            Count = data.Count;
        }

        protected override void OnPrepare(GameObject entity)
        {
        }

        protected override void OnClaim(GameObject entity)
        {
            entity.GetComponent<AttributesComponent>().Points += Count;
        }
    }
}