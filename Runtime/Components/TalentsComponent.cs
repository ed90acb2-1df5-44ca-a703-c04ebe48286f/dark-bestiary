using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Talents;

namespace DarkBestiary.Components
{
    public class TalentsComponent : Component
    {
        public event Action<TalentsComponent, Talent> AnyTalentLearned;
        public event Action<TalentsComponent, Talent> AnyTalentUnlearned;
        public event Action<TalentsComponent> PointsChanged;

        public List<TalentTier> Tiers { get; private set; }

        private int m_Points;

        public int Points
        {
            get => m_Points;
            set
            {
                m_Points = value;
                PointsChanged?.Invoke(this);
            }
        }

        public TalentsComponent Construct(List<Talent> talents, IEnumerable<int> learned, int points)
        {
            Tiers = new List<TalentTier>();

            foreach (var category in talents.GroupBy(t => t.Category.Id))
            {
                foreach (var tier in category.OrderBy(t => t.Tier).GroupBy(t => t.Tier))
                {
                    var first = tier.First();

                    Tiers.Add(
                        new TalentTier(
                            first.Category,
                            first.Tier,
                            tier.OrderBy(talent => talent.Index).ToList(),
                            first.Tier == 1
                        )
                    );
                }
            }

            Points = points;

            foreach (var talentId in learned)
            {
                Learn(talentId, false);
            }

            return this;
        }

        protected override void OnInitialize()
        {
            foreach (var tier in Tiers)
            {
                foreach (var talent in tier.Talents)
                {
                    talent.Learned += OnTalentLearned;
                    talent.Unlearned += OnTalentUnlearned;
                }
            }
        }

        protected override void OnTerminate()
        {
            foreach (var tier in Tiers)
            {
                foreach (var talent in tier.Talents)
                {
                    talent.Learned -= OnTalentLearned;
                    talent.Unlearned -= OnTalentUnlearned;
                }
            }
        }

        public void Learn(int talentId, bool spendPoints = true)
        {
            var tier = Tiers.FirstOrDefault(tt => tt.Talents.Any(t => t.Id == talentId));

            if (tier == null)
            {
                return;
            }

            foreach (var learned in tier.Talents)
            {
                Unlearn(learned);
            }

            if (spendPoints)
            {
                if (Points == 0)
                {
                    return;
                }

                Points--;
            }

            tier.Talents.FirstOrDefault(t => t.Id == talentId)?.Learn(gameObject);
            Tiers.FirstOrDefault(tt => tt.Category.Id == tier.Category.Id && tt.Index == tier.Index + 1)?.Unlock();
        }

        public void Unlearn(int talentId)
        {
            var tier = Tiers.FirstOrDefault(tt => tt.Talents.Any(t => t.Id == talentId));

            if (tier == null)
            {
                return;
            }

            Unlearn(tier.Talents.FirstOrDefault(t => t.Id == talentId));

            foreach (var nextTier in Tiers.Where(t => t.Category.Id == tier.Category.Id && t.Index > tier.Index))
            {
                foreach (var talent in nextTier.Talents)
                {
                    Unlearn(talent);
                }

                nextTier.Lock();
            }
        }

        private void Unlearn(Talent talent)
        {
            if (talent == null || !talent.IsLearned)
            {
                return;
            }

            Points++;
            talent.Unlearn(gameObject);
        }

        public void UnlearnAll()
        {
            foreach (var tier in Tiers)
            {
                foreach (var talent in tier.Talents)
                {
                    talent.Unlearn(gameObject);
                }

                if (tier.Index > 1)
                {
                    tier.Lock();
                }
            }
        }

        private void OnTalentUnlearned(Talent talent)
        {
            AnyTalentUnlearned?.Invoke(this, talent);
        }

        private void OnTalentLearned(Talent talent)
        {
            AnyTalentLearned?.Invoke(this, talent);
        }
    }
}