using System.Collections;
using DarkBestiary.Managers;
using EZCameraShake;
using UnityEngine;

namespace DarkBestiary
{
    public class CameraShakeEffect : MonoBehaviour
    {
        [SerializeField] private float delay;
        [SerializeField] private float magnitude;
        [SerializeField] private float roughness;
        [SerializeField] private float fadeInTime;
        [SerializeField] private float fadeOutTime;

        private void OnEnable()
        {
            if (SettingsManager.Instance.DisableCameraShake)
            {
                return;
            }

            StartCoroutine(Shake(this.delay));
        }

        private IEnumerator Shake(float duration)
        {
            yield return new WaitForSeconds(duration);
            CameraShaker.Instance.ShakeOnce(this.magnitude, this.roughness, this.fadeInTime, this.fadeOutTime);
        }
    }
}