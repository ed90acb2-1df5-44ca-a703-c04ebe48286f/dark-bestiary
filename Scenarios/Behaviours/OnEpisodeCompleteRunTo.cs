using UnityEngine;

namespace DarkBestiary.Scenarios.Behaviours
{
    public class OnEpisodeCompleteRunTo : MonoBehaviour
    {
        [SerializeField] private Vector3 destination;
        [SerializeField] private float delay;

        private bool isRunning;

        private void Start()
        {
            Episode.AnyEpisodeCompleted += OnAnyEpisodeCompleted;
        }

        private void OnDestroy()
        {
            Episode.AnyEpisodeCompleted -= OnAnyEpisodeCompleted;
        }

        private void OnAnyEpisodeCompleted(Episode episode)
        {
            Episode.AnyEpisodeCompleted -= OnAnyEpisodeCompleted;

            Timer.Instance.Wait(this.delay, () =>
            {
                this.destination = GetDestination();
                this.isRunning = true;

                var model = GetComponent<Model>();
                model.LookAt(this.destination);
                model.PlayAnimation("walk");
            });
        }

        protected virtual Vector3 GetDestination()
        {
            return this.destination;
        }

        private void Update()
        {
            if (!this.isRunning)
            {
                return;
            }

            if ((this.destination - transform.position).magnitude <= 0.1f)
            {
                var model = GetComponent<Model>();
                model.FadeOut(0.5f);
                model.PlayAnimation("idle");
                this.isRunning = false;
            }

            transform.position += Time.deltaTime * 4f * (this.destination - transform.position).normalized;
        }
    }
}