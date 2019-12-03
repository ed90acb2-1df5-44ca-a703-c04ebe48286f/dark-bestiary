using DarkBestiary.GameBoard;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(Board))]
    public class BoardEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(25);

            if (GUILayout.Button("Generate"))
            {
                ((Board) target).Generate();
            }
        }
    }
}
