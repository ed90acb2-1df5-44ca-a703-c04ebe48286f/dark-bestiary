using System;

namespace DarkBestiary.Scenarios.Encounters
{
    public abstract class Encounter
    {
        public static event Action<Encounter> AnyEncounterStarted;
        public static event Action<Encounter> AnyEncounterCompleted;
        public static event Action<Encounter> AnyEncounterFailed;

        public event Action<Encounter> Started;
        public event Action<Encounter> Completed;
        public event Action<Encounter> Failed;

        public static bool IsCombat => CombatEncounter.Active != null;

        public bool IsCompleted { get; private set; }
        public bool IsFailed { get; private set; }
        public bool IsCompletedOrFailed => IsCompleted || IsFailed;

        public void Start()
        {
            OnStart();

            Started?.Invoke(this);
            AnyEncounterStarted?.Invoke(this);
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