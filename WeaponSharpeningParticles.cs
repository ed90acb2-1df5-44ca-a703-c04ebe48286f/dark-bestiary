using System.Collections.Generic;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary
{
    public class WeaponSharpeningParticles : MonoBehaviour
    {
        private static readonly Color White = new Color(0.8f, 0.8f, 0.8f);
        private static readonly Color Blue = new Color(0.25f, 0.25f, 1);
        private static readonly Color Purple = new Color(0.6f, 0.2f, 0.9f);
        private static readonly Color Orange = new Color(1, 0.3f, 0);
        private static readonly Color Red = new Color(1, 0, 0);

        private static readonly Dictionary<int, Color> SharpeningColors = new Dictionary<int, Color>()
        {
            {1,  White.With(a: 0.25f)},
            {2,  White.With(a: 0.5f)},
            {3,  White.With(a: 0.75f)},
            {4,  White.With(a: 1f)},

            {5,  Blue.With(a: 0.25f)},
            {6,  Blue.With(a: 0.5f)},
            {7,  Blue.With(a: 0.75f)},
            {8,  Blue.With(a: 1f)},

            {9,  Purple.With(a: 0.5f)},
            {10, Purple.With(a: 0.75f)},
            {11, Purple.With(a: 1f)},

            {12, Orange.With(a: 0.5f)},
            {13, Orange.With(a: 0.75f)},
            {14, Orange.With(a: 1f)},

            {15, Red.With(a: 1f)},
        };

        [SerializeField] private ParticleSystem lightning;
        [SerializeField] private ParticleSystem smoke;
        [SerializeField] private ParticleSystem sparks;
        [SerializeField] private ParticleSystem halo;

        private Item item;

        public void Construct(Item item)
        {
            this.item = item;
            this.item.SharpeningLevelChanged += OnSharpeningLevelChanged;

            OnSharpeningLevelChanged(item);
        }

        private void OnDestroy()
        {
            this.item.SharpeningLevelChanged -= OnSharpeningLevelChanged;
        }

        private void OnSharpeningLevelChanged(Item item)
        {
            this.smoke.gameObject.SetActive(item.SharpeningLevel >= 1);
            this.sparks.gameObject.SetActive(item.SharpeningLevel >= 5);
            this.lightning.gameObject.SetActive(item.SharpeningLevel >= 8);
            this.halo.gameObject.SetActive(item.SharpeningLevel >= 10);

            if (this.item.SharpeningLevel == 0)
            {
                return;
            }

            SetParticleSystemColor(this.smoke, SharpeningColors[this.item.SharpeningLevel]);
            SetParticleSystemColor(this.sparks, SharpeningColors[this.item.SharpeningLevel]);
            SetParticleSystemColor(this.lightning, SharpeningColors[this.item.SharpeningLevel]);
            SetParticleSystemColor(this.halo, SharpeningColors[this.item.SharpeningLevel]);
        }

        private static void SetParticleSystemColor(ParticleSystem particleSystem, Color color)
        {
            var main = particleSystem.main;
            main.startColor = color;
        }
    }
}