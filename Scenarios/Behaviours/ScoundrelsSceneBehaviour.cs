using System.Collections;
using DarkBestiary.Data;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.Scenarios.Behaviours
{
    public class ScoundrelsSceneBehaviour : MonoBehaviour
    {
        [SerializeField] private Model hunter;
        [SerializeField] private Model[] scoundrels;

        private void Start()
        {
            TalkEncounter.AnyPhraseShown += OnAnyPhraseShown;
        }

        private void OnDestroy()
        {
            TalkEncounter.AnyPhraseShown -= OnAnyPhraseShown;
        }

        private void OnAnyPhraseShown(PhraseData phrase)
        {
            if (phrase.Label != "Scenario_Scoundrels_01_13")
            {
                return;
            }

            this.hunter.PlayAnimation("walk_scare");
            StartCoroutine(MoveTo(this.hunter, new Vector3(2, 6)));

            foreach (var scoundrel in this.scoundrels)
            {
                Timer.Instance.Wait(Random.Range(1, 1.5f), () =>
                {
                    scoundrel.PlayAnimation("walk");
                    StartCoroutine(MoveTo(scoundrel, scoundrel.transform.position + new Vector3(Random.Range(0, 10), 14)));
                });
            }
        }

        private IEnumerator MoveTo(Model model, Vector3 destination)
        {
            while ((destination - model.transform.position).magnitude > 0.1f)
            {
                model.transform.position += Time.deltaTime * 5f * (destination - model.transform.position).normalized;
                yield return null;
            }

            model.PlayAnimation("idle");
        }
    }
}