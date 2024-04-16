using System;

namespace StageLight.FixtureListView
{
    [Serializable]
    internal class SearchState
    {
        public int SearchFilter { get; set; } = -1;
        public string SearchString { get; set; } = "";

        /// <summary> フィルタ条件をリセット </summary>
        public void ResetSearch()
        {
            SearchString = "";
            SearchFilter = -1;
        }

        /// <summary> 条件に一致するか </summary>
        public bool IsMatch(ColumnType column, FixtureTreeElement element)
        {
            var value = element.GetColumnValue(column);
            return value switch
            {
                string s => IsMatchValue(SearchString, s),
                int i => IsMatchValue(SearchString, i),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Undefined value type")
            };
        }

        /// <summary> 検索文字列が一致するか </summary>
        private static bool IsMatchValue(string searchText, string displayText)
        {
            // 何も表示されていない場合は true
            if (string.IsNullOrEmpty(displayText)) { return true; }

            return displayText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsMatchValue(string searchText, int displayValue)
        {
            if (string.IsNullOrEmpty(searchText)) return true;

            return searchText == displayValue.ToString();
        }
    }
}
