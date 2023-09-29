using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(ThreatComponent))]
    public class ThreatComponentEditor : UnityEditor.Editor
    {
        private ThreatComponent m_Threat;
        private IEnumerable<KeyValuePair<GameObject, float>> m_Enemies;

        private void OnEnable()
        {
            m_Threat = (ThreatComponent) target;
            SortEnemies();
        }

        private void SortEnemies()
        {
            m_Enemies = m_Threat.Table.OrderByDescending(pair => pair.Value);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Time.frameCount % 200 == 0)
            {
                SortEnemies();
            }

            foreach (var pair in m_Enemies)
            {
                EditorGUILayout.LabelField($"{pair.Key.name} - {pair.Value:F2}");
            }
        }
    }
}
