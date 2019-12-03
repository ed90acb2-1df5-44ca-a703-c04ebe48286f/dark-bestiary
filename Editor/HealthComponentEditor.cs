using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(HealthComponent))]
    public class HealthComponentEditor : UnityEditor.Editor
    {
        private HealthComponent health;

        private void OnEnable()
        {
            this.health = target as HealthComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.FloatField("Health Max", this.health.HealthMax);
            EditorGUILayout.FloatField("Current Health", this.health.Health);
            EditorGUILayout.FloatField("Current Shield", this.health.Shield);
            EditorGUILayout.LabelField("Alive", this.health.IsAlive ? "Yes" : "No");
        }
    }
}
