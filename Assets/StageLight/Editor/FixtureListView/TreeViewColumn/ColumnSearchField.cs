using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace StageLight.FixtureListView
{
    internal class ColumnSearchField
    {
        private SearchField SearchField { get; } = new();

        /// <summary> 列の検索ボックスを描画 </summary>
        public void OnGUI(Rect searchRect, FixtureColumnHeaderState headerState, int columnIndex)
        {
            var searchState = headerState.SearchStates[columnIndex];
            var column = HeaderConfig.HeaderColumnTypes[columnIndex];

            switch (column)
            {
                case ColumnType.FixtureName:
                case ColumnType.ObjectName:
                case ColumnType.ParentName:
                case ColumnType.StartAddress:
                    searchState.SearchString = SearchField.OnToolbarGUI(searchRect, searchState.SearchString);
                    break;
                case ColumnType.Universe:
                case ColumnType.ChannelMode:
                    // TODO: EditorGUI.MaskField でフィルタを行う
                    searchState.SearchString = SearchField.OnToolbarGUI(searchRect, searchState.SearchString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(column), column, "Unknown column index");
            }
        }
    }
}
