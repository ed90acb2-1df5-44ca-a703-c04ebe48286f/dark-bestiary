using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.Rewards
{
    public class RandomSkillsUnlockReward : Reward
    {
        public List<Skill> Skills { get; private set; }

        private readonly RandomSkillsUnlockRewardData data;
        private readonly ISkillRepository skillRepository;

        public RandomSkillsUnlockReward(RandomSkillsUnlockRewardData data, ISkillRepository skillRepository)
        {
            this.data = data;
            this.skillRepository = skillRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            var spellbook = entity.GetComponent<SpellbookComponent>();

            Skills = this.skillRepository.Tradable(s => !spellbook.IsKnown(s.Id))
                .Shuffle()
                .Take(this.data.Count)
                .ToList();
        }

        protected override void OnClaim(GameObject entity)
        {
            entity.GetComponent<SpellbookComponent>().Add(Skills);
        }
    }
}