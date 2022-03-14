using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using DarkBestiary.UI.Views.Unity;
using UnityEngine;

namespace DarkBestiary.Scenarios.Encounters
{
    public class TalkEncounter : Encounter
    {
        public static event Payload<PhraseData> AnyPhraseShown;

        private readonly ITalkEncounterView view;
        private readonly Queue<PhraseData> phrases;

        private NarratorComponent[] narrators;

        public TalkEncounter(ITalkEncounterView view, IEnumerable<PhraseData> phrases)
        {
            this.view = view;
            this.view.Hide();

            this.phrases = new Queue<PhraseData>(phrases.OrderBy(p => p.Label));
        }

        protected override void OnStart()
        {
            if (ActionBarView.Active != null)
            {
                ActionBarView.Active.gameObject.SetActive(false);
            }

            SpeechBubble.Instance.Hidden += OnContinue;

            this.narrators = Object.FindObjectsOfType<NarratorComponent>();

            this.view.Initialize();
            this.view.Show();

            Timer.Instance.Wait(1, () =>
            {
                this.view.Continue += OnContinue;
                OnContinue();
            });
        }

        protected override void OnStop()
        {
            if (IsCompleted)
            {
                return;
            }

            OnComplete();
        }

        protected override void OnComplete()
        {
            SpeechBubble.Instance.Hidden -= OnContinue;
            SpeechBubble.Instance.Hide();

            if (ActionBarView.Active != null)
            {
                ActionBarView.Active.gameObject.SetActive(true);
            }

            this.view.Terminate();
        }

        private void OnContinue()
        {
            if (this.phrases.Count == 0)
            {
                SpeechBubble.Instance.Hide();
                Complete();
                return;
            }

            var phrase = this.phrases.Dequeue();
            var narrator = this.narrators.First(n => n.Narrator == phrase.Narrator);

            SpeechBubble.Instance.Show(I18N.Instance.Get(phrase.TextKey), narrator.transform);

            AnyPhraseShown?.Invoke(phrase);
        }
    }
}