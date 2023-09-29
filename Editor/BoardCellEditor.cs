using DarkBestiary.GameBoard;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(BoardCell))]
    public class BoardCellEditor : UnityEditor.Editor
    {
        private BoardCell m_Cell;

        private void OnEnable()
        {
            m_Cell = target as BoardCell;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var gameObject in m_Cell.GameObjectsInside)
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
