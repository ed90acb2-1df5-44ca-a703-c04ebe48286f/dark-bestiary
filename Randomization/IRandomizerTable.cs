using System.Collections.Generic;

namespace DarkBestiary.Randomization
{
    public interface IRandomizerTable : IRandomizerObject
    {
        int Count { get; set; }

        List<IRandomizerObject> Contents { get; }

        List<IRandomizerObject> Evaluate();
    }
}