using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;

namespace DarkBestiary.Randomization
{
    public class RandomizerTable : RandomizerObject, IRandomizerTable
    {
        public int Count { get; set; }
        public List<IRandomizerObject> Contents { get; }

        private readonly List<IRandomizerObject> unique = new List<IRandomizerObject>();

        public RandomizerTable(int count =1,
            float probability = 0,
            bool unique = true,
            bool guaranteed = true,
            bool enabled = true) : base(probability, unique, guaranteed, enabled)
        {
            Contents = new List<IRandomizerObject>();
            Count = count;
        }

        public void Add(IRandomizerObject entry)
        {
            entry.Table = this;
            Contents.Add(entry);
        }

        public void Add(IEnumerable<IRandomizerObject> entries)
        {
            foreach (var entry in entries)
            {
                Add(entry);
            }
        }

        public List<IRandomizerObject> Evaluate()
        {
            this.unique.Clear();

            var result = new List<IRandomizerObject>();

            var enabled = Contents.Where(item => item.Enabled).ToList();
            var guaranteed = enabled.Where(item => item.Guaranteed).ToList();

            foreach (var item in guaranteed)
            {
                PopulateResult(result, item);
            }

            var unguaranteedCount = Count - guaranteed.Count;

            if (unguaranteedCount <= 0)
            {
                return result;
            }

            var unguaranteed = enabled.Where(item => !item.Guaranteed).ToList();

            unguaranteed = unguaranteed.DistinctBy(value => value.Probability).Count() == 1
                ? unguaranteed.Shuffle().ToList()
                : unguaranteed.OrderBy(item => item.Probability).ToList();

            var probabilitySum = unguaranteed.Sum(item => item.Probability);

            for (var i = 0; i < unguaranteedCount; i++)
            {
                var growingProbability = 0f;

                foreach (var item in unguaranteed)
                {
                    item.OnCheck();

                    growingProbability += item.Probability;

                    if (RNG.Float() * probabilitySum > growingProbability)
                    {
                        continue;
                    }

                    PopulateResult(result, item);
                    break;
                }
            }

            return result;
        }

        private void PopulateResult(List<IRandomizerObject> result, IRandomizerObject entry)
        {
            if (entry is RandomizerNullValue || entry.Unique && this.unique.Contains(entry))
            {
                return;
            }

            entry.OnHit();

            this.unique.Add(entry);

            if (entry is IRandomizerTable table)
            {
                foreach (var randomizerObject in table.Evaluate())
                {
                    PopulateResult(result, randomizerObject);
                }

                return;
            }

            result.Add(entry);
        }
    }
}