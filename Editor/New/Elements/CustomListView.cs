using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DarkBestiary.Editor.New.Elements
{
    public class CustomListView : VisualElement
    {
        public void Setup<T>(IReadOnlyCollection<T> elements, Func<T, string> getLabel, Func<T, string> getIcon, Action<T> onSelectionChanged)
        {
            const string markupPath = "Assets/DarkBestiary/UI_Toolkit/Elements/CustomListView.uxml";
            const string stylesheetPath = "Assets/DarkBestiary/UI_Toolkit/Elements/CustomListView.uss";

            var filteredElements = new List<T>();

            Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(markupPath).CloneTree());
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheetPath));

            var listView = this.Q<ListView>();
            listView.makeItem = MakeItem;
            listView.bindItem = BindItem;
            listView.itemsSource = filteredElements;
            listView.selectionType = SelectionType.Single;
            listView.selectionChanged += OnListViewSelectionChanged;

            var textField = this.Q<TextField>();
            textField.RegisterCallback<ChangeEvent<string>>(OnTextFieldChanged);
            textField.value = "";

            FilterElements("");

            return;

            void BindItem(VisualElement element, int index)
            {
                element.Q<Label>().text = getLabel(filteredElements[index]);
                element.Q<Image>().sprite = Resources.Load<Sprite>(getIcon(filteredElements[index]));
            }

            VisualElement MakeItem()
            {
                var container = new VisualElement();
                container.AddToClassList("element");
                container.Add(new Image());
                container.Add(new Label());
                return container;
            }

            void OnTextFieldChanged(ChangeEvent<string> e)
            {
                FilterElements(e.newValue);
            }

            void OnListViewSelectionChanged(IEnumerable<object> objects)
            {
                onSelectionChanged((T) objects.First());
            }

            void FilterElements(string value)
            {
                filteredElements.Clear();
                filteredElements.AddRange(
                    elements.Where(x => string.IsNullOrEmpty(value) || getLabel(x).Contains(value))
                );

                listView.RefreshItems();
            }
        }

        public new class UxmlFactory : UxmlFactory<CustomListView, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
    }
}