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
        private ThreatComponent threat;
        private IEnumerable<KeyValuePair<GameObject, float>> enemies;

        private void OnEnable()
        {
            this.threat = (ThreatComponent) target;
            SortEnemies();
        }

        private void SortEnemies()
        {
            this.enemies = this.threat.Table.OrderByDescending(pair => pair.Value);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Time.frameCount % 200 == 0)
            {
                SortEnemies();
            }

            foreach (var pair in this.enemies)
            {
                EditorGUILayout.LabelField($"{pair.Key.name} - {pair.Value:F2}");
            }
        }
    }
}
