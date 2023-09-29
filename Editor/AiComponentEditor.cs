using System;
using System.Linq;
using DarkBestiary.AI;
using DarkBestiary.Components;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(AiComponent))]
    public class EntityEditor : UnityEditor.Editor
    {
        private AiComponent m_AI;

        private void OnEnable()
        {
            m_AI = target as AiComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(25);
            EditorGUILayout.LabelField("Combat", m_AI.Tree.Context.Combat != null ? "True" : "False");
            EditorGUILayout.LabelField("Target Entity", m_AI.Tree.Context.TargetEntity == null ? "Null" : m_AI.Tree.Context.TargetEntity.name);
            EditorGUILayout.LabelField("Target Point", m_AI.Tree.Context.TargetPoint.HasValue ? m_AI.Tree.Context.TargetPoint.Value.ToString() : "Null");
            EditorGUILayout.LabelField("Opened Nodes", string.Join(", ", m_AI.Tree.Context.OpenedNodes.Select(node => node.GetType().Name)));
            GUILayout.Space(25);
            DisplayParentNode(m_AI.Tree, m_AI.Tree.Root, 0);
            GUILayout.Space(25);
        }

        private void DisplayParentNode(BehaviourTree tree, IBehaviourTreeNodeParent node, int level)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                padding = new RectOffset(10 * level, 0, 0, 0),
                fontStyle = tree.GetRunningNode().Equals(node) ? FontStyle.Bold : FontStyle.Normal
            };

            GUILayout.Label($"<color=#{GetColorByStatus(node.LastStatus)}>{node.GetType().Name}</color>", style);

            foreach (var child in node.Children)
            {
                if (child is IBehaviourTreeNodeParent subParent)
                {
                    DisplayParentNode(tree, subParent, level + 1);
                    continue;
                }

                style.padding = new RectOffset(10 * (level + 1), 0, 0, 0);
                style.fontStyle = tree.GetRunningNode().Equals(child) ? FontStyle.Bold : FontStyle.Normal;

                GUILayout.Label($"<color=#{GetColorByStatus(child.LastStatus)}>{child.GetType().Name}</color>", style);
            }
        }

        private static string GetColorByStatus(BehaviourTreeStatus status)
        {
            switch (status)
            {
                case BehaviourTreeStatus.Success:
                    return "008800";
                case BehaviourTreeStatus.Failure:
                    return "880000";
                case BehaviourTreeStatus.Running:
                    return "000088";
                case BehaviourTreeStatus.None:
                    return "333333";
                case BehaviourTreeStatus.Waiting:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return "ff00ff";
        }
    }
}
