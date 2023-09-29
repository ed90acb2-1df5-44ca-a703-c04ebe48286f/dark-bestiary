using System.Linq;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class AnimatorExtensions
    {
        public static float GetAnimationClipLength(this Animator animator, string name)
        {
            return animator.GetAnimationClip(name)?.length ?? 0;
        }

        public static AnimationClip GetAnimationClip(this Animator animator, string name)
        {
            return animator.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == name);
        }
    }
}