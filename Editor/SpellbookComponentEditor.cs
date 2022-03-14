using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(SpellbookComponent))]
    public class SpellbookComponentEditor : UnityEditor.Editor
    {
        private SpellbookComponent spellbook;

        private void OnEnable()
        {
            this.spellbook = target as SpellbookComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var slot in this.spellbook.Slots)
            {
                EditorGUILayout.LabelField($"#{slot.Index} {slot.Skill.Name} {(slot.Skill.IsOnCooldown() ? $" (Cooldown {slot.Skill.RemainingCooldown})" : "" )}");
            }
        }
    }
}
