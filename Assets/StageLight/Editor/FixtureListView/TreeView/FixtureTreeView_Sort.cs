using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace StageLight.FixtureListView
{
    internal partial class FixtureTreeView
    {
        private static Func<FixtureTreeElement, object> GetSortSelector(ColumnType column)
        {
            return l => l.GetColumnValue(column);
        }

        private static void TreeToList(TreeViewItem root, ICollection<TreeViewItem> result)
        {
            if (root == null) throw new NullReferenceException("root");
            if (result == null) throw new NullReferenceException("result");

            result.Clear();

            if (root.children == null) return;

            var stack = new Stack<TreeViewItem>();
            for (var i = root.children.Count - 1; i >= 0; i--)
            {
                stack.Push(root.children[i]);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop() as FixtureTreeViewItem;
                result.Add(current);

                if (current is not { hasChildren: true } || current.children[0] == null) continue;
                for (var i = current.children.Count - 1; i >= 0; i--)
                {
                    stack.Push(current.children[i]);
                }
            }
        }

        private void OnSortingChanged(MultiColumnHeader header)
        {
            SortIfNeeded(rootItem, GetRows());
        }

        private void SortIfNeeded(TreeViewItem root, ICollection<TreeViewItem> rows)
        {
            if (rows.Count <= 1) return;
            if (multiColumnHeader.sortedColumnIndex == -1) return;

            SortByMultipleColumns();
            TreeToList(root, rows);
            Repaint();
        }


        private void SortByMultipleColumns()
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;
            if (sortedColumns.Length == 0) return;

            var searchStates = (multiColumnHeader.state as FixtureColumnHeaderState)?.SearchStates;
            // 検索条件に一致する要素を取得
            var children = GetRows()
                .Cast<FixtureTreeViewItem>()
                .Where(l => l.Data.IsMatchSearch(searchStates))
                .ToArray();

            var orderedQuery = InitialOrder(children, sortedColumns);
            for (var i = 1; i < sortedColumns.Length; i++)
            {
                var sortOption = HeaderConfig.HeaderColumnTypes[sortedColumns[i]];
                var ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

                orderedQuery = orderedQuery.ThenBy(l => GetSortSelector(sortOption)(l.Data), ascending);
            }

            rootItem.children = orderedQuery
                .Cast<TreeViewItem>()
                .ToList();
        }

        private IOrderedEnumerable<FixtureTreeViewItem> InitialOrder(IEnumerable<FixtureTreeViewItem> elements,
            IReadOnlyList<int> history)
        {
            var columnIndex = history[0];
            var column = HeaderConfig.HeaderColumnTypes[columnIndex];
            var ascending = multiColumnHeader.IsSortedAscending(columnIndex);
            return elements.Order(l => GetSortSelector(column)(l.Data), ascending);
        }
    }
}
