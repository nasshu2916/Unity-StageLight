using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace StageLight.FixtureListView
{
    internal class FixtureColumnHeader : MultiColumnHeader
    {
        private const float SearchHeight = 18f; // 検索ボックスの高さ
        private const float SearchMargin = 4f;
        private const float SortHeight = 4f; // ソートボタンの高さ
        private const float LabelHeight = 28f; // ラベルの高さ
        private const float LabelMargin = 4f;

        public FixtureColumnHeader(MultiColumnHeaderState state) : base(state)
        {
            height = SearchHeight + SearchMargin + LabelHeight + LabelMargin;
        }

        /// <summary> 検索条件が変化したときの Callback </summary>
        public Action SearchChanged { get; set; }

        /// <summary> ヘッダー描画 </summary>
        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            headerRect.y += SearchMargin;
            headerRect.height -= SearchMargin;

            var searchRect = new Rect(headerRect)
            {
                height = SearchHeight
            };
            searchRect.width -= SearchMargin * 2f;
            searchRect.x += SearchMargin;

            EditorGUI.BeginChangeCheck();
            var headerState = state as FixtureColumnHeaderState;
            Debug.Assert(headerState != null, nameof(headerState) + " != null");
            var searchField = headerState.SearchFields[columnIndex];
            searchField.OnGUI(searchRect, headerState, columnIndex);
            if (EditorGUI.EndChangeCheck())
            {
                SearchChanged?.Invoke();
            }

            if (canSort && column.canSort)
            {
                var sortRect = headerRect;
                sortRect.height = LabelHeight + LabelMargin;
                sortRect.y = searchRect.height + SortHeight;
                SortingButton(column, sortRect, columnIndex);
            }

            var labelRect = new Rect(headerRect.x,
                headerRect.yMax - LabelHeight - LabelMargin,
                headerRect.width,
                LabelHeight);
            GUI.Label(labelRect, column.headerContent, MyStyle.TreeViewColumnHeader);
        }
    }
}
