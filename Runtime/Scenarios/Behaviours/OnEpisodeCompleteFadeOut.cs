using UnityEngine;

namespace DarkBestiary.Scenarios.Behaviours
{
    public class OnEpisodeCompleteFadeOut : MonoBehaviour
    {
        private void Start()
        {
            Episode.AnyEpisodeCompleted += OnAnyEpisodeCompleted;
        }

        private void OnAnyEpisodeCompleted(Episode episode)
        {
            Episode.AnyEpisodeCompleted -= OnAnyEpisodeCompleted;
            GetComponent<Model>().FadeOut(2);
        }
    }
}