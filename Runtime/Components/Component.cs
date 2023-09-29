using System;
using UnityEngine;

namespace DarkBestiary.Components
{
    public abstract class Component : MonoBehaviour
    {
        public static event Action<Component> AnyComponentInitialized;
        public static event Action<Component> AnyComponentTerminated;

        public event Action<Component> Initialized;
        public event Action<Component> Terminated;

        public void Initialize()
        {
            OnInitialize();

            Initialized?.Invoke(this);
            AnyComponentInitialized?.Invoke(this);
        }

        public void Terminate()
        {
            OnTerminate();

            Terminated?.Invoke(this);
            AnyComponentTerminated?.Invoke(this);
        }

        private void Update()
        {
            OnTick(Time.deltaTime);
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnTerminate()
        {
        }

        protected virtual void OnTick(float delta)
        {
        }
    }
}