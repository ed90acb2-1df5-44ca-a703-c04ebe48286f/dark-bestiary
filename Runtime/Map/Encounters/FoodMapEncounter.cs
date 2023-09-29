using System;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.Map.Encounters
{
    public class FoodMapEncounter : IMapEncounter
    {
        private Action? m_OnSuccess;
        private EateryViewController? m_EateryController;

        public void Run(Action onSuccess, Action onFailure)
        {
            m_OnSuccess = onSuccess;

            m_EateryController = Container.Instance.Instantiate<EateryViewController>();
            m_EateryController.Initialize();
            m_EateryController.View.Hidden += OnEateryViewHidden;
            m_EateryController.View.RequiresConfirmationOnClose = true;
            m_EateryController.View.Show();
        }

        public void Cleanup()
        {
            if (m_EateryController == null)
            {
                return;
            }

            m_EateryController.View.Hidden -= OnEateryViewHidden;
            m_EateryController.Terminate();
        }

        private void OnEateryViewHidden()
        {
            m_OnSuccess?.Invoke();
        }
    }
}