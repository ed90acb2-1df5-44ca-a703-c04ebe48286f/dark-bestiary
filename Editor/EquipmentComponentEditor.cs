using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(EquipmentComponent))]
    public class EquipmentComponentEditor : UnityEditor.Editor
    {
        private EquipmentComponent equipment;

        private void OnEnable()
        {
            this.equipment = target as EquipmentComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var slot in this.equipment.Slots)
            {
                EditorGUILayout.LabelField($"{slot.Type}: {slot.Item.Name}");
            }
        }
    }
}
