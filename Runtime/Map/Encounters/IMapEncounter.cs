using System;

namespace DarkBestiary.Map.Encounters
{
    public interface IMapEncounter
    {
        void Run(Action onSuccess, Action onFailure);

        void Cleanup();
    }
}