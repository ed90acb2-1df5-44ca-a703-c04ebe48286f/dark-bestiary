using DarkBestiary.Components;
using DarkBestiary.Managers;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(CombatManager))]
    public class CombatManagerEditor : UnityEditor.Editor
    {
        private CombatManager m_CombatManager;

        private void OnEnable()
        {
            m_CombatManager = target as CombatManager;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_CombatManager.Combat == null)
            {
                EditorGUILayout.LabelField("Not in combat");
                return;
            }

            EditorGUILayout.LabelField($"Round: {m_CombatManager.Combat.RoundNumber}");

            var acting = m_CombatManager.Combat.Acting == null
                ? "null"
                : $"{m_CombatManager.Combat.Acting.GetComponent<UnitComponent>().Name} ({m_CombatManager.Combat.Acting.name})";

            EditorGUILayout.LabelField($"Acting: {acting})");
            EditorGUILayout.LabelField("Queue:");

            foreach (var entity in m_CombatManager.Combat.Queue)
            {
                EditorGUILayout.LabelField($"    {entity.GetComponent<UnitComponent>().Name} ({entity.name})");
            }
        }
    }
}
