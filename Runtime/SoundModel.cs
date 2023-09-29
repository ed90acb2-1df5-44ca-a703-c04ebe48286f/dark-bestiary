using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary
{
    [Serializable]
    public class AnimationSoundInfo
    {
        public float Frame;
        public string Animation;
        [FMODUnity.EventRef] public string Sound;
    }

    public class SoundModel : MonoBehaviour
    {
        [SerializeField] private List<AnimationSoundInfo> m_Sounds = new();

        private void Start()
        {
            var actor = GetComponentInParent<ActorComponent>();

            if (actor == null)
            {
                return;
            }

            actor.AnimationPlayed += OnAnimationPlayed;
        }

        private void OnAnimationPlayed(string animation)
        {
            var animationSoundInfo = m_Sounds.FirstOrDefault(s => s.Animation == animation);

            if (animationSoundInfo == null)
            {
                return;
            }

            Timer.Instance.Wait(animationSoundInfo.Frame * (1f / 60),
                () => { AudioManager.Instance.PlayOneShot(animationSoundInfo.Sound); });
        }
    }
}