using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(SpellbookComponent))]
    public class SpellbookComponentEditor : UnityEditor.Editor
    {
        private SpellbookComponent m_Spellbook;

        private void OnEnable()
        {
            m_Spellbook = target as SpellbookComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var slot in m_Spellbook.Slots)
            {
                EditorGUILayout.LabelField($"#{slot.Index} {slot.Skill.Name} {(slot.Skill.IsOnCooldown() ? $" (Cooldown {slot.Skill.RemainingCooldown})" : "" )}");
            }
        }
    }
}
