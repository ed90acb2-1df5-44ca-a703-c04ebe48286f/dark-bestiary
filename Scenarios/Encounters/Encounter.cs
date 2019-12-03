using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.Scenarios.Encounters
{
    public abstract class Encounter
    {
        public static event Payload<Encounter> AnyEncounterStarted;
        public static event Payload<Encounter> AnyEncounterCompleted;
        public static event Payload<Encounter> AnyEncounterFailed;

        public event Payload<Encounter> Started;
        public event Payload<Encounter> Completed;
        public event Payload<Encounter> Failed;

        public static bool IsCombat => CombatEncounter.Active != null;

        public PhraseData StartPhrase { get; set; }
        public PhraseData CompletePhrase { get; set; }

        public bool IsCompleted { get; private set; }
        public bool IsFailed { get; private set; }
        public bool IsCompletedOrFailed => IsCompleted || IsFailed;

        public void Start()
        {
            OnStart();

            Started?.Invoke(this);
            AnyEncounterStarted?.Invoke(this);

            MaybeShowStartPhrase();
        }

        public void Stop()
        {
            OnStop();
        }

        protected void Complete()
        {
            if (IsCompleted)
            {
                return;
            }

            OnComplete();

            if (CompletePhrase == null)
            {
                TriggerCompleted();
                return;
            }

            var narrator = Object.FindObjectsOfType<NarratorComponent>()
                .FirstOrDefault(n => n.Narrator == CompletePhrase.Narrator);

            if (narrator == null)
            {
                TriggerCompleted();
                return;
            }

            var text = I18N.Instance.Get(CompletePhrase.TextKey).ToString();

            if (!Scenario.Active.IsActiveEpisodeLast)
            {
                SpeechBubble.Instance.Show(text, narrator.transform, text.CalculateReadTime());
                TriggerCompleted();
            }
            else
            {
                SpeechBubble.Instance.Show(text, narrator.transform, text.CalculateReadTime());
                SpeechBubble.Instance.Hidden += OnSpeechBubbleHidden;
            }
        }

        private void MaybeShowStartPhrase()
        {
            if (StartPhrase == null)
            {
                return;
            }

            var narrator = Object.FindObjectsOfType<NarratorComponent>()
                .FirstOrDefault(n => n.Narrator == StartPhrase.Narrator);

            if (narrator == null)
            {
                return;
            }

            var text = I18N.Instance.Get(StartPhrase.TextKey).ToString();

            SpeechBubble.Instance.Show(text, narrator.transform, Mathf.Max(1, text.CalculateReadTime()));
        }

        private void OnSpeechBubbleHidden()
        {
            SpeechBubble.Instance.Hidden -= OnSpeechBubbleHidden;
            TriggerCompleted();
        }

        private void TriggerCompleted()
        {
            if (IsCompleted)
            {
                return;
            }

            IsCompleted = true;

            Completed?.Invoke(this);
            AnyEncounterCompleted?.Invoke(this);
        }

        protected void Fail()
        {
            OnFail();

            IsFailed = true;

            Failed?.Invoke(this);
            AnyEncounterFailed?.Invoke(this);
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        protected virtual void OnComplete()
        {
        }

        protected virtual void OnFail()
        {
        }

        public virtual void Tick(float delta)
        {
        }
    }
}