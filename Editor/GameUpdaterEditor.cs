using DarkBestiary.Managers;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(GameUpdater))]
    public class GameUpdaterEditor : UnityEditor.Editor
    {
        private GameUpdater m_Updater;
        private bool m_IsUpdating;
        private string m_Progress;

        private void OnEnable()
        {
            m_Updater = (GameUpdater) target;
            m_Updater.UpdateStarted += OnUpdateStarted;
            m_Updater.UpdateCompleted += OnUpdateCompleted;
            m_Updater.UpdateProgress += OnUpdateProgress;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(25);

            if (m_IsUpdating)
            {
                GUILayout.Label(m_Progress);
            }
            else
            {
                if (GUILayout.Button("Update"))
                {
                    m_Updater.StartUpdate();
                }
            }
        }

        private void OnUpdateStarted()
        {
            m_IsUpdating = true;
        }

        private void OnUpdateCompleted()
        {
            m_IsUpdating = false;
        }

        private void OnUpdateProgress(string progress)
        {
            m_Progress = progress;
        }
    }
}
