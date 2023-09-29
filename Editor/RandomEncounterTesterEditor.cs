using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Managers;
using DarkBestiary.Testers;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(RandomEncounterTester))]
    public class RandomEncounterTesterEditor : UnityEditor.Editor
    {
        private List<UnitData> m_Units = new List<UnitData>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(25);

            if (!Application.isPlaying )
            {
                GUILayout.Label("Requires Play mode");
                return;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Test"))
            {
                m_Units = ((RandomEncounterTester) target).Test();
            }

            if (GUILayout.Button("Clear"))
            {
                m_Units.Clear();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(25);

            if (m_Units.Count == 0)
            {
                GUILayout.Label("...");
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Encounter:");

            foreach (var group in m_Units.GroupBy(unit => unit.Id))
            {
                GUILayout.Label($"    {I18N.Instance.Get(group.First().NameKey)} x{group.Count()}");
            }

            GUILayout.Space(25);
        }
    }
}