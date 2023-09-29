using DarkBestiary.Components;
using UnityEditor;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(BehavioursComponent))]
    public class BehavioursComponentEditor : UnityEditor.Editor
    {
        private BehavioursComponent m_Behaviours;

        private void OnEnable()
        {
            m_Behaviours = target as BehavioursComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var behaviour in m_Behaviours.Behaviours)
            {
                var label = behaviour.Name.IsNullOrEmpty() ? behaviour.Label : behaviour.Name;

                EditorGUILayout.LabelField(
                    " #" + behaviour.Id + " " + label + " " + behaviour.GetType() +
                    " x" + behaviour.StackCount + $" ({behaviour.RemainingDuration}/{behaviour.Duration})");
            }
        }
    }
}
