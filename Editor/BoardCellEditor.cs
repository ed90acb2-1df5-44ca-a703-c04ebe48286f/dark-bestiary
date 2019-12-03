using DarkBestiary.GameBoard;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(BoardCell))]
    public class BoardCellEditor : UnityEditor.Editor
    {
        private BoardCell cell;

        private void OnEnable()
        {
            this.cell = target as BoardCell;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var gameObject in this.cell.GameObjectsInside)
            {
                if (gameObject == null)
                {
                    continue;
                }

                EditorGUILayout.LabelField(gameObject.name);
            }
        }
    }
}
