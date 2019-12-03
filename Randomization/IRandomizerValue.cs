namespace DarkBestiary.Randomization
{
    public interface IRandomizerValue<out TValue> : IRandomizerObject
    {
        TValue Value { get; }
    }
}