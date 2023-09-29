using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Testers;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(LootDropTester))]
    public class LootDropTesterEditor : UnityEditor.Editor
    {
        private List<Item> m_Items = new List<Item>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(25);

            if (!Application.isPlaying )
            {
                GUILayout.Label("Requires Play mode");
                return;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Test"))
            {
                m_Items = ((LootDropTester) target).Test();
            }

            if (GUILayout.Button("Clear"))
            {
                m_Items.Clear();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(25);

            if (m_Items.Count == 0)
            {
                GUILayout.Label("...");
            }

            foreach (var byRarity in m_Items.OrderBy(item => item.Rarity.Type).ThenBy(item => item.Name).GroupBy(item => item.Rarity.Id))
            {
                GUILayout.Space(10);

                GUILayout.Label($"{byRarity.First().Rarity.Name} - {byRarity.Count()}");

                foreach (var group in byRarity.GroupBy(item => item.Id))
                {
                    GUILayout.Label($"    {group.First().Name} x{group.Sum(item => item.StackCount)}");
                }
            }

            GUILayout.Space(25);
        }
    }
}