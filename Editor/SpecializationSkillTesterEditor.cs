using DarkBestiary.Testers;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(SpecializationSkillTester))]
    public class SpecializationSkillTesterEditor : UnityEditor.Editor
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
                ((SpecializationSkillTester) target).Test();
            }
        }
    }
}
