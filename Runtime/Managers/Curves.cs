using UnityEngine;

namespace DarkBestiary.Managers
{
    public class Curves : Singleton<Curves>
    {
        public AnimationCurve Parabolic;
        public AnimationCurve Logarithmic;
        public AnimationCurve Exponential;
        public AnimationCurve Linear;
    }
}