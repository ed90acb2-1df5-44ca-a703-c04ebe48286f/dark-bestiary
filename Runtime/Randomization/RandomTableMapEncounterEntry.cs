using DarkBestiary.Data;

namespace DarkBestiary.Randomization
{
    public class RandomTableMapEncounterEntry : RandomTableEntry<MapEncounterData>
    {
        public RandomTableMapEncounterEntry(MapEncounterData value, RandomTableEntryParameters parameters) : base(value, parameters)
        {
        }

        public override bool Equals(object? other)
        {
            return other is RandomTableMapEncounterEntry mapEncounter && Value.Equals(mapEncounter.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}