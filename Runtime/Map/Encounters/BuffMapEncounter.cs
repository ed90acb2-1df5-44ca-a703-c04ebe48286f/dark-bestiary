using System;
using DarkBestiary.Components;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.Map.Encounters
{
    public class BuffMapEncounter : IMapEncounter
    {
        private BuffSelectionViewController? m_BuffSelectionController;

        private Action? m_OnSuccess;

        public void Run(Action onSuccess, Action onFailure)
        {
            m_OnSuccess = onSuccess;

            m_BuffSelectionController = Container.Instance.Instantiate<BuffSelectionViewController>();
            m_BuffSelectionController.Initialize();
            m_BuffSelectionController.View.Show();
            m_BuffSelectionController.View.Hidden += OnBuffSelectionViewHidden;
        }

        public void Cleanup()
        {
            if (m_BuffSelectionController == null)
            {
                return;
            }

            m_BuffSelectionController.View.Hidden -= OnBuffSelectionViewHidden;
            m_BuffSelectionController.Terminate();
        }

        private void OnBuffSelectionViewHidden()
        {
            var behavioursComponent = Game.Instance.Character.Entity.GetComponent<BehavioursComponent>();
            behavioursComponent.ApplyAllStacks(m_BuffSelectionController!.SelectedBuff, behavioursComponent.gameObject);

            m_OnSuccess?.Invoke();
        }
    }
}