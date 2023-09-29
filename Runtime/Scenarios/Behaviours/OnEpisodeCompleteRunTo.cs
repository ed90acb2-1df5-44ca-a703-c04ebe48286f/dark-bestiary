using UnityEngine;

namespace DarkBestiary.Scenarios.Behaviours
{
    public class OnEpisodeCompleteRunTo : MonoBehaviour
    {
        [SerializeField] private Vector3 m_Destination;
        [SerializeField] private float m_Delay;

        private bool m_IsRunning;

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

            Timer.Instance.Wait(m_Delay, () =>
            {
                m_Destination = GetDestination();
                m_IsRunning = true;

                var model = GetComponent<Model>();
                model.LookAt(m_Destination);
                model.PlayAnimation("walk");
            });
        }

        protected virtual Vector3 GetDestination()
        {
            return m_Destination;
        }

        private void Update()
        {
            if (!m_IsRunning)
            {
                return;
            }

            if ((m_Destination - transform.position).magnitude <= 0.1f)
            {
                var model = GetComponent<Model>();
                model.FadeOut(0.5f);
                model.PlayAnimation("idle");
                m_IsRunning = false;
            }

            transform.position += Time.deltaTime * 4f * (m_Destination - transform.position).normalized;
        }
    }
}