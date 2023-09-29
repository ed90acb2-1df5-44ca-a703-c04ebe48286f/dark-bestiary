using System;

namespace DarkBestiary.Map.Encounters
{
    public class SkillSelectMapEncounter : IMapEncounter
    {
        private readonly MapEncounterRunner m_Context;

        private Action? m_OnSuccess;
        private Action? m_OnFailure;

        public SkillSelectMapEncounter(MapEncounterRunner context)
        {
            m_Context = context;
        }

        public void Run(Action onSuccess, Action onFailure)
        {
            m_OnSuccess = onSuccess;
            m_OnFailure = onFailure;

            m_Context.SkillSelectViewController.Completed += OnSkillSelectViewControllerCompleted;
            m_Context.SkillRemoveViewController.Completed += OnSkillRemoveViewControllerCompleted;
            m_Context.SkillRemoveViewController.Cancelled += OnSkillRemoveViewControllerCancelled;

            ShowRequiredView();
        }

        public void Cleanup()
        {
            m_Context.SkillSelectViewController.Completed -= OnSkillSelectViewControllerCompleted;
            m_Context.SkillRemoveViewController.Completed -= OnSkillRemoveViewControllerCompleted;
            m_Context.SkillRemoveViewController.Cancelled -= OnSkillRemoveViewControllerCancelled;
        }

        private void ShowRequiredView()
        {
            if (m_Context.SkillRemoveViewController.HasReachedMaxNumberOfSkills())
            {
                m_Context.SkillRemoveViewController.ShowView();
                m_Context.SkillRemoveViewController.RefreshSkills();
            }
            else
            {
                m_Context.SkillSelectViewController.ShowView();
                m_Context.SkillSelectViewController.RefreshSkills();
            }
        }

        private void OnSkillRemoveViewControllerCompleted()
        {
            ShowRequiredView();
        }

        private void OnSkillRemoveViewControllerCancelled()
        {
            m_OnFailure?.Invoke();
        }

        private void OnSkillSelectViewControllerCompleted()
        {
            m_OnSuccess?.Invoke();
        }
    }
}