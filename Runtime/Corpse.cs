using System;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary
{
    public class Corpse : MonoBehaviour
    {
        public static event Action<Corpse> AnyCorpseConsumed;

        public bool IsConsumed { get; private set; }

        [SerializeField] private GameObject m_DeathPrefab;

        private void Start()
        {
            foreach (var element in GetComponentsInChildren<Renderer>())
            {
                element.FadeIn(1.0f);
            }
        }

        public void Consume()
        {
            AnyCorpseConsumed?.Invoke(this);

            Instantiate(m_DeathPrefab, transform.position, Quaternion.identity).DestroyAsVisualEffect();

            IsConsumed = true;

            foreach (var element in GetComponentsInChildren<Renderer>())
            {
                element.FadeOut(1.0f);
            }

            Timer.Instance.Wait(1, () => Destroy(gameObject));
        }
    }
}