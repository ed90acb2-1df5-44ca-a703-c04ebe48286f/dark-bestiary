using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(InventoryComponent))]
    public class InventoryComponentEditor : UnityEditor.Editor
    {
        private InventoryComponent m_Inventory;

        private void OnEnable()
        {
            m_Inventory = target as InventoryComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            for (var i = 0; i < m_Inventory.Items.Count; i++)
            {
                EditorGUILayout.LabelField(
                    i + " " + (m_Inventory.Items[i] != null ? m_Inventory.Items[i].Name : "NULL")
                );
            }
        }
    }
}
