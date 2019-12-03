using DarkBestiary.Managers;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(GameUpdater))]
    public class GameUpdaterEditor : UnityEditor.Editor
    {
        private GameUpdater updater;
        private bool isUpdating;
        private string progress;

        private void OnEnable()
        {
            this.updater = (GameUpdater) target;
            this.updater.UpdateStarted += OnUpdateStarted;
            this.updater.UpdateCompleted += OnUpdateCompleted;
            this.updater.UpdateProgress += OnUpdateProgress;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(25);

            if (this.isUpdating)
            {
                GUILayout.Label(this.progress);
            }
            else
            {
                if (GUILayout.Button("Update"))
                {
                    this.updater.StartUpdate();
                }
            }
        }

        private void OnUpdateStarted()
        {
            this.isUpdating = true;
        }

        private void OnUpdateCompleted()
        {
            this.isUpdating = false;
        }

        private void OnUpdateProgress(string progress)
        {
            this.progress = progress;
        }
    }
}
