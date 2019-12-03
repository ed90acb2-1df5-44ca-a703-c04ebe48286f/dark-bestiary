using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.Components
{
    public abstract class Component : MonoBehaviour
    {
        public static event Payload<Component> AnyComponentInitialized;
        public static event Payload<Component> AnyComponentTerminated;

        public event Payload<Component> Initialized;
        public event Payload<Component> Terminated;

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