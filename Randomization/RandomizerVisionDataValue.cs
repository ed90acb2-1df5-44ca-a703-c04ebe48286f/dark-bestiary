using DarkBestiary.Data;

namespace DarkBestiary.Randomization
{
    public class RandomizerVisionDataValue : RandomizerValue<VisionData>
    {
        public RandomizerVisionDataValue(VisionData value, float probability) : base(value, probability)
        {
        }

        public RandomizerVisionDataValue(VisionData value, float probability, bool unique, bool guaranteed, bool enabled) : base(value, probability, unique, guaranteed, enabled)
        {
        }

        public override bool Equals(object other)
        {
            if (!(other is RandomizerVisionDataValue otherValue))
            {
                return false;
            }

            return Value.Equals(otherValue.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}