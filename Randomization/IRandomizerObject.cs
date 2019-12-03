namespace DarkBestiary.Randomization
{
    public interface IRandomizerObject
    {
        IRandomizerTable Table { get; set; }
        float Probability { get; set; }
        bool Unique { get; set; }
        bool Guaranteed { get; set; }
        bool Enabled { get; set; }

        void OnCheck();

        void OnHit();
    }
}