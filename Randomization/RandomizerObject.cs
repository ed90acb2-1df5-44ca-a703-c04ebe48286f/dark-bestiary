namespace DarkBestiary.Randomization
{
    public abstract class RandomizerObject : IRandomizerObject
    {
        public IRandomizerTable Table { get; set; }
        public float Probability { get; set; }
        public bool Unique { get; set; }
        public bool Guaranteed { get; set; }
        public bool Enabled { get; set; }

        protected RandomizerObject(float probability, bool unique, bool guaranteed, bool enabled)
        {
            Probability = probability;
            Unique = unique;
            Guaranteed = guaranteed;
            Enabled = enabled;
        }

        public virtual void OnCheck()
        {
        }

        public virtual void OnHit()
        {
        }
    }
}