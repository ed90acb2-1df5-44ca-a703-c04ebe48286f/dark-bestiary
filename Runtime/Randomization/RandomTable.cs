using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Randomization
{
    public class RandomTable : RandomTableEntry
    {
        private readonly int m_Count;
        private readonly List<RandomTableEntry> m_Entries = new();

        private readonly List<RandomTableEntry> m_Unique = new();
        private readonly List<RandomTableEntry> m_Result = new();

        public RandomTable() : this(1)
        {
        }

        public RandomTable(int count, RandomTableEntryParameters parameters = default) : base(parameters)
        {
            m_Count = count;
        }

        public void AddEntry(RandomTableEntry entry)
        {
            m_Entries.Add(entry);
        }

        public IEnumerable<RandomTableEntry> Evaluate()
        {
            m_Unique.Clear();
            m_Result.Clear();

            foreach (var entry in m_Entries.Where(x => x.IsEnabled && x.IsGuaranteed))
            {
                AddResult(entry);
            }

            var unguaranteedCount = m_Count - m_Entries.Count(x => x.IsEnabled && x.IsGuaranteed);

            if (unguaranteedCount <= 0)
            {
                return m_Result;
            }

            var probabilitySum = m_Entries.Where(x => x.IsEnabled && x.IsGuaranteed == false).Sum(x => x.Weight);

            var hits = 0;
            var iterations = 0;

            while (hits < unguaranteedCount && iterations < unguaranteedCount * 2)
            {
                var growingProbability = 0f;
                var random = Rng.Float();

                foreach (var current in m_Entries.Where(x => x.IsEnabled && x.IsGuaranteed == false).OrderByDescending(x => x.Weight))
                {
                    var entry = current;

                    if (entry is RandomTableRandomItemEntry randomItemEntry)
                    {
                        entry = randomItemEntry.Roll();

                        if (entry == null)
                        {
                            continue;
                        }
                    }

                    if (IsUniqueAndAlreadyAdded(entry))
                    {
                        continue;
                    }

                    growingProbability += entry.Weight;

                    if (random * probabilitySum >= growingProbability)
                    {
                        continue;
                    }

                    hits += 1;
                    AddResult(entry);
                    break;
                }

                iterations += 1;
            }

            return m_Result;
        }

        private bool IsUniqueAndAlreadyAdded(RandomTableEntry item)
        {
            return item.IsUnique && m_Unique.Any(x => x.Equals(item));
        }

        private void AddResult(RandomTableEntry entry)
        {
            if (entry is RandomTableNullEntry || IsUniqueAndAlreadyAdded(entry))
            {
                return;
            }

            if (entry is RandomTable table)
            {
                foreach (var randomizerObject in table.Evaluate())
                {
                    AddResult(randomizerObject);
                }

                return;
            }

            if (entry.IsUnique)
            {
                m_Unique.Add(entry);
            }

            m_Result.Add(entry);
        }
    }
}