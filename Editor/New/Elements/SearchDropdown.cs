using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace DarkBestiary.Editor.New.Elements
{
    public class SearchDropdown : VisualElement
    {
        public SearchDropdown()
        {
            const string markupPath = "Assets/DarkBestiary/UI_Toolkit/Elements/SearchDropdown.uxml";
            const string stylesheetPath = "Assets/DarkBestiary/UI_Toolkit/Elements/SearchDropdown.uss";

            Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(markupPath).CloneTree());
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheetPath));

            var choices = new List<string> { "null", "Option 1", "Option 2", "Option 3" };
            var dropdownField = this.Q<DropdownField>();
            dropdownField.choices = choices;
            dropdownField.index = 0;
        }

        public new class UxmlFactory : UxmlFactory<SearchDropdown, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
    }
}