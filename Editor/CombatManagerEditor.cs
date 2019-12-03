using DarkBestiary.Components;
using DarkBestiary.Managers;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(CombatManager))]
    public class CombatManagerEditor : UnityEditor.Editor
    {
        private CombatManager combatManager;

        private void OnEnable()
        {
            this.combatManager = target as CombatManager;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (this.combatManager.Combat == null)
            {
                EditorGUILayout.LabelField("Not in combat");
                return;
            }

            EditorGUILayout.LabelField($"Round: {this.combatManager.Combat.RoundNumber}");

            var acting = this.combatManager.Combat.Acting == null
                ? "null"
                : $"{this.combatManager.Combat.Acting.GetComponent<UnitComponent>().Name} ({this.combatManager.Combat.Acting.name})";

            EditorGUILayout.LabelField($"Acting: {acting})");
            EditorGUILayout.LabelField("Queue:");

            foreach (var entity in this.combatManager.Combat.Queue)
            {
                EditorGUILayout.LabelField($"    {entity.GetComponent<UnitComponent>().Name} ({entity.name})");
            }
        }
    }
}
