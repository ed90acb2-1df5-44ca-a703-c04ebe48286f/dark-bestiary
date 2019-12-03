using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(InventoryComponent))]
    public class InventoryComponentEditor : UnityEditor.Editor
    {
        private InventoryComponent inventory;

        private void OnEnable()
        {
            this.inventory = target as InventoryComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            for (var i = 0; i < this.inventory.Items.Count; i++)
            {
                EditorGUILayout.LabelField(
                    i + " " + (this.inventory.Items[i] != null ? this.inventory.Items[i].Name : "NULL")
                );
            }
        }
    }
}
