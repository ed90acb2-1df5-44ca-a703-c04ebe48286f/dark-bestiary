using DarkBestiary.Managers;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(ScenarioExperienceTester))]
    public class ScenarioExperienceTesterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(25);

            if (!Application.isPlaying )
            {
                GUILayout.Label("Requires Play mode");
                return;
            }

            if (GUILayout.Button("Test"))
            {
                ((ScenarioExperienceTester) target).Test();
            }
        }
    }
}
