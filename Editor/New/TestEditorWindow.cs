using System.Collections.Generic;
using System.IO;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Editor.New.Controllers;
using DarkBestiary.Editor.New.Elements;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DarkBestiary.Editor.New
{
    public class TestEditorWindow : EditorWindow
    {
        private VisualElement m_Form;
        private List<ItemData> m_Items;

        //[MenuItem("Data Editor/Test")]
        public static void CreateWindow()
        {
            var window = GetWindow<TestEditorWindow>();
            window.titleContent = new GUIContent("Test Editor Window");
        }

        public void CreateGUI()
        {
            Initialize();

            var root = rootVisualElement;

            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DarkBestiary/UI_Toolkit/Views/Test.uxml").CloneTree(root);
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DarkBestiary/UI_Toolkit/Views/Test.uss"));
            root.Q<CustomListView>().Setup(m_Items, GetLabel, GetIcon, OnSelectionChanged);

            CreateForm(root);
        }

        public void Initialize()
        {
            m_Items = JsonConvert.DeserializeObject<List<ItemData>>(File.ReadAllText(Environment.s_StreamingAssetsPath + "/compiled/data/items.json"));
        }

        private void CreateForm(VisualElement root)
        {
            m_Form = root.Q<VisualElement>("form");
            m_Form.Add(CreateField("Id"));
            m_Form.Add(CreateField("Key"));

            var objectField = new ObjectField("Icon");
            objectField.name = "Icon";
            objectField.objectType = typeof(Sprite);
            m_Form.Add(objectField);

            var behaviourTreeView = new TreeView();
            m_Form.Add(behaviourTreeView);

            var button = new Button();
            button.text = "Save";
            m_Form.Add(button);
        }

        private void OnSelectionChanged(ItemData data)
        {
            m_Form.Q<TextField>("Id").value = data.Id.ToString();
            m_Form.Q<TextField>("Key").value = data.NameKey;
            m_Form.Q<ObjectField>("Icon").value = Resources.Load<Sprite>(data.Icon);

            var behaviourTree = JsonConvert.DeserializeObject<List<BehaviourTreeData>>(File.ReadAllText(Environment.s_StreamingAssetsPath + "/compiled/data/ai_2.json")).First(x => x.Id == 1373);
            new BehaviourTreeViewController(behaviourTree, m_Form.Q<TreeView>());

            Debug.Log(AssetDatabase.GetAssetPath(Resources.Load<Sprite>(data.Icon)));
        }

        private static string GetIcon(ItemData data)
        {
            return data.Icon;
        }

        private static string GetLabel(ItemData data)
        {
            return data.NameKey;
        }

        private static VisualElement CreateField(string name)
        {
            var visualElement = new TextField(name);
            visualElement.name = name;
            visualElement.isReadOnly = true;
            return visualElement;
        }
    }
}