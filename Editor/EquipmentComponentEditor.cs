using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(EquipmentComponent))]
    public class EquipmentComponentEditor : UnityEditor.Editor
    {
        private EquipmentComponent m_Equipment;

        private void OnEnable()
        {
            m_Equipment = target as EquipmentComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var slot in m_Equipment.Slots)
            {
                EditorGUILayout.LabelField($"{slot.Type}: {slot.Item.Name}");
            }
        }
    }
}
