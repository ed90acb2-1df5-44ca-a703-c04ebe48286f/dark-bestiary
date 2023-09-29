using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(HealthComponent))]
    public class HealthComponentEditor : UnityEditor.Editor
    {
        private HealthComponent m_Health;

        private void OnEnable()
        {
            m_Health = target as HealthComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.FloatField("Health Max", m_Health.HealthMax);
            EditorGUILayout.FloatField("Current Health", m_Health.Health);
            EditorGUILayout.FloatField("Current Shield", m_Health.Shield);
            EditorGUILayout.LabelField("Alive", m_Health.IsAlive ? "Yes" : "No");
        }
    }
}
