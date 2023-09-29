using System.Collections.Generic;
using DarkBestiary.Data;
using UnityEngine.UIElements;

namespace DarkBestiary.Editor.New.Controllers
{
    public class BehaviourTreeViewController
    {
        private readonly TreeView m_BehaviourTreeView;
        private readonly BehaviourTreeData m_BehaviourTree;

        public BehaviourTreeViewController(BehaviourTreeData behaviourTree, TreeView behaviourTreeView)
        {
            m_BehaviourTree = behaviourTree;

            m_BehaviourTreeView = behaviourTreeView;
            m_BehaviourTreeView.makeItem = MakeItem;
            m_BehaviourTreeView.bindItem = BindItem;
            m_BehaviourTreeView.selectionType = SelectionType.Single;
            m_BehaviourTreeView.selectedIndicesChanged += chosen => { };

            RebuildBehaviourTreeView();
        }

        private VisualElement MakeItem()
        {
            var visualElement = new Label();
            return visualElement;
        }

        private void BindItem(VisualElement element, int index)
        {
            var item = m_BehaviourTreeView.GetItemDataForIndex<BehaviourTreeData>(index);
            var label = (Label) element;

            label.text = item.Type;

            CreateContextMenu(label, item, index);
        }

        private void CreateContextMenu(VisualElement label, BehaviourTreeData item, int index)
        {
            var contextMenu = new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Add", action =>
                {
                    item.Children.Add(new BehaviourTreeData()
                    {
                        Type = "New"
                    });

                    RebuildBehaviourTreeView();
                });

                evt.menu.AppendAction("Remove", action =>
                {
                    var parentId = m_BehaviourTreeView.GetParentIdForIndex(index);
                    var parent = m_BehaviourTreeView.GetItemDataForId<BehaviourTreeData>(parentId);

                    if (parent == null)
                    {
                        m_BehaviourTreeView.SetRootItems(new List<TreeViewItemData<BehaviourTreeData>>());
                        m_BehaviourTreeView.Rebuild();
                        return;
                    }

                    parent.Children.Remove(item);

                    RebuildBehaviourTreeView();
                });
            });

            contextMenu.target = label.parent;
        }

        public void RebuildBehaviourTreeView()
        {
            var items = new List<TreeViewItemData<BehaviourTreeData>>();
            CreateTreeViewItemData(items, m_BehaviourTree);

            m_BehaviourTreeView.SetRootItems(items);
            m_BehaviourTreeView.Rebuild();
            m_BehaviourTreeView.ExpandAll();
        }

        private static void CreateTreeViewItemData(ICollection<TreeViewItemData<BehaviourTreeData>> list, BehaviourTreeData root)
        {
            foreach (var child in root.Children)
            {
                var grandchildren = new List<TreeViewItemData<BehaviourTreeData>>();

                CreateTreeViewItemData(grandchildren, child);

                list.Add(new TreeViewItemData<BehaviourTreeData>(child.GetHashCode(), child, grandchildren));
            }
        }
    }
}