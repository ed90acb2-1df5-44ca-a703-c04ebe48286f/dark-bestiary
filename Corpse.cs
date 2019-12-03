using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary
{
    public class Corpse : MonoBehaviour
    {
        public static event Payload<Corpse> AnyCorpseConsumed;

        public bool IsConsumed { get; private set; }

        [SerializeField] private GameObject deathPrefab;

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

            Instantiate(this.deathPrefab, transform.position, Quaternion.identity).DestroyAsVisualEffect();

            IsConsumed = true;

            foreach (var element in GetComponentsInChildren<Renderer>())
            {
                element.FadeOut(1.0f);
            }

            Timer.Instance.Wait(1, () => Destroy(gameObject));
        }
    }
}