using System;
using System.Collections.Generic;
using System.Linq;
using StageLight.DmxFixture;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace StageLight.FixtureListView
{
    internal partial class FixtureTreeView : TreeView
    {
        public const string DefaultSearchString = "HOGE";
        private const TextAnchor FieldLabelAnchor = TextAnchor.MiddleLeft;

        private FixtureTreeElement[] _baseElements = Array.Empty<FixtureTreeElement>(); // TreeViewで描画する要素

        public FixtureTreeView(FixtureTreeViewState state, FixtureColumnHeaderState headerState)
            : base(state, new FixtureColumnHeader(headerState))
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;

            if (multiColumnHeader is not FixtureColumnHeader textureColumnHeader)
            {
                throw new NullReferenceException("textureColumnHeader");
            }

            textureColumnHeader.sortingChanged += OnSortingChanged; // ソート変化時の処理を登録
            textureColumnHeader.SearchChanged += CallSearchChanged; // 列の検索が変化したときの処理を登録
        }

        private void DrawRowColumn(RowGUIArgs args, Rect rect, int columnIndex)
        {
            if (args.item.id < 0) { return; }

            var element = _baseElements[args.item.id];
            if (element == null) { return; }

            var column = HeaderConfig.HeaderColumnTypes[columnIndex];

            switch (column)
            {
                default:
                    var text = element.GetDisplayText(column);
                    var style = element.GetLabelStyle(column);
                    EditorGUI.LabelField(rect, text, style);
                    break;
            }
        }

        public void Clean()
        {
            _baseElements = Array.Empty<FixtureTreeElement>();
            Reload();
        }

        /// <summary> 検索にヒットするかどうか </summary>
        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            var textureItem = item as FixtureTreeViewItem;
            var textureHeaderState = multiColumnHeader.state as FixtureColumnHeaderState;
            return textureItem!.Data.IsMatchSearch(textureHeaderState!.SearchStates);
        }

        /// <summary> 列の作成する </summary>
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            // BuildRows()で返されたIListを元にしてTreeView上で描画する
            var rows = GetRows() ?? new List<TreeViewItem>();
            rows.Clear();

            //　TreeViewItemの親子関係を構築
            foreach (var baseElement in _baseElements)
            {
                var baseItem = new FixtureTreeViewItem
                {
                    id = baseElement.Id, displayName = baseElement.Fixture.Name, Data = baseElement
                };

                // 検索にヒットする場合は追加
                if (!DoesItemMatchSearch(baseItem, searchString)) continue;

                root.AddChild(baseItem);
                rows.Add(baseItem);
            }

            SetupDepthsFromParentsAndChildren(root);
            return rows;
        }


        /// <summary> ルートの作成 </summary>
        protected override TreeViewItem BuildRoot()
        {
            // Rootだけを返す
            return new FixtureTreeViewItem
            {
                id = -1, depth = -1, displayName = "Root"
            };
        }

        /// <summary> TreeView初期化 </summary>
        public void SetFixtures((GameObject gameObject, IDmxFixture fixture)[] fixtures)
        {
            fixtures = fixtures.OrderBy(tuple =>
            {
                var parent = tuple.gameObject.transform.parent;
                return parent != null ? parent.name : "";
            }).ThenBy(tuple => tuple.fixture.Name).ThenBy(tuple => tuple.gameObject.name).ToArray();

            _baseElements = fixtures.Select((tuple, index) => new FixtureTreeElement(tuple.gameObject, tuple.fixture)
            {
                Id = index
            }).ToArray();
        }

        /// <summary> TreeViewの列の描画 </summary>
        protected override void RowGUI(RowGUIArgs args)
        {
            // TreeView 各列の描画
            for (var visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); visibleColumnIndex++)
            {
                var rect = args.GetCellRect(visibleColumnIndex);
                var columnIndex = args.GetColumn(visibleColumnIndex);
                var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;
                labelStyle.alignment = FieldLabelAnchor;

                DrawRowColumn(args, rect, columnIndex);
            }
        }

        private void CallSearchChanged()
        {
            // searchString の値を変更し TreeView に検索文字列が変更されたことを通知する
            searchString = "";
            searchString = DefaultSearchString;
        }


        /// <summary> 選択中の要素を取得 </summary>
        private IEnumerable<FixtureTreeElement> GetSelectionElement()
        {
            return GetSelection().Select(index => _baseElements[index]);
        }

        #region Override Event

        protected override void KeyEvent()
        {
            base.KeyEvent();

            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
            {
                // TODO: エンターキーを押したときの処理を追加
            }
        }

        /// <summary> コンテキストメニューのクリック時の処理 </summary>
        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);

            var fixtureNames = GetSelectionElement().Select(e => e.Fixture.Name).ToArray();
            // TODO: コンテキストメニューの処理を追加
            Debug.Log($"ContextClickedItem: {string.Join(",", fixtureNames)}");
        }

        /// <summary> クリック時の処理 </summary>
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            var obj = _baseElements[id].GameObject;
            EditorGUIUtility.PingObject(obj);
        }

        /// <summary> ダブルクリック時の処理 </summary>
        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var obj = _baseElements[id].GameObject;

            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }

        #endregion
    }
}
