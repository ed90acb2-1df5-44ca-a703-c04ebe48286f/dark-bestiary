using UnityEngine;

namespace DarkBestiary.Visuals
{
    public interface ILightning
    {
        void Initialize(Vector3 origin, Vector3 target);

        void Destroy(float delay);

        void Destroy();

        void FadeOut(float duration);

        void FadeOut(float delay, float duration);
    }
}