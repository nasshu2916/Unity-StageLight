using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace StageLight.FixtureListView
{
    internal class FixtureColumnHeaderState : MultiColumnHeaderState
    {
        public FixtureColumnHeaderState(Column[] columns, SearchState[] searchStates) : base(columns)
        {
            SearchStates = searchStates;
            SearchFields = columns.Select(c => new ColumnSearchField()).ToArray();
        }
        public ColumnSearchField[] SearchFields { get; } // 検索ボックス
        public SearchState[] SearchStates { get; } // 検索状態

        /// <summary> 検索条件をリセットする </summary>
        public void ResetSearch()
        {
            foreach (var state in SearchStates)
            {
                state.ResetSearch();
            }
        }
    }
}
