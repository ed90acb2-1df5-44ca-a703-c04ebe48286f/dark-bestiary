using System.IO;
using System.Linq;
using DarkBestiary.Managers;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    [CustomEditor(typeof(FontReplacer))]
    public class FontReplacerEditor : UnityEditor.Editor
    {
        private FontReplacer replacer;

        private void OnEnable()
        {
            this.replacer = target as FontReplacer;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Replace"))
            {
                ReplaceInFolder("/UI/Prefabs");
                ReplaceInFolder("/Resources");
            }
        }

        private void ReplaceInFolder(string folder)
        {
            var prefabs = Directory.GetFiles(
                Application.dataPath + folder, "*.prefab", SearchOption.AllDirectories);

            foreach (var prefab in prefabs)
            {
                var path = prefab.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/");
                Replace(path);
            }
        }

        private void Replace(string path)
        {
            var @object = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (@object == null)
            {
                return;
            }

            var components = @object.GetComponentsInChildren<TextMeshProUGUI>();

            if (components.All(c => c.font != this.replacer.From))
            {
                return;
            }

            foreach (var text in components)
            {
                if (text.font == this.replacer.From)
                {
                    text.font = this.replacer.To;
                }
            }

            Debug.Log(@object.name);

            PrefabUtility.SavePrefabAsset(@object);
        }
    }
}