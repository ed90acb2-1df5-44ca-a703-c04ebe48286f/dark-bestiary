using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class KeyFrameExtensions
    {
        public static Keyframe With(
            this Keyframe original,
            float? time = null,
            float? value = null,
            float? inTangent = null,
            float? outTangent = null,
            float? inWeight = null,
            float? outWeight = null,
            WeightedMode? weightedMode = null)
        {
            var keyframe = new Keyframe(
                time ?? original.time,
                value ?? original.value,
                inTangent ?? original.inTangent,
                outTangent ?? original.outTangent,
                inWeight ?? original.inWeight,
                outWeight ?? original.outWeight
            );

            keyframe.weightedMode = weightedMode ?? original.weightedMode;

            return keyframe;
        }
    }
}