using System;
using System.Linq;
using StageLight.DmxFixture;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace StageLight.FixtureListView
{
    /// <summary> Scene 内の Fixture 一覧ツール </summary>
    internal class FixtureViewerWindow : EditorWindow
    {
        [SerializeField] private SearchState[] _columnSearchStates = Array.Empty<SearchState>(); // 検索状態 (列)

        private (GameObject gameObject, IDmxFixture fixture)[] _fixtures; // フィクスチャ一覧
        private FixtureColumnHeaderState _headerState; // TreeViewヘッダー状態
        private bool _isCreatingTreeView;

        private bool _isLoadingFixtures;
        private SearchField _searchField; // 検索窓
        private FixtureTreeView _treeView; // TreeView
        private FixtureTreeViewState _treeViewState; // TreeViewの状態

        private void OnGUI()
        {
            if (_treeView == null)
            {
                CreateTreeView();
            }

            DrawHeader();
            DrawTreeView();
        }

        [MenuItem("ArtNet/Fixture Viewer")]
        private static void OpenWindow()
        {
            var window = GetWindow<FixtureViewerWindow>();
            window.titleContent = new GUIContent("Texture Viewer");

            var position = window.position;
            position.width = HeaderConfig.InitialHeaderTotalWidth + 50f;
            position.height = 400f;
            window.position = position;

            window.CreateTreeView(); // 起動直後にTreeView作成
        }

        /// <summary> TreeViewを描画 </summary>
        private void DrawTreeView()
        {
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.ExpandHeight(true));
            _treeView?.OnGUI(rect);
        }

        /// <summary> ウィンドウ上部のヘッダー描画 </summary>
        private void DrawHeader()
        {
            var defaultColor = GUI.backgroundColor;

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUI.backgroundColor = Color.green;
                DrawReloadButton();
                GUI.backgroundColor = defaultColor;

                GUILayout.Space(100);

                GUILayout.FlexibleSpace();
            }

            GUI.backgroundColor = defaultColor;
        } // ReSharper disable Unity.PerformanceAnalysis
        private void DrawReloadButton()
        {
            if (_treeView == null) { return; }

            if (!GUILayout.Button("Reload", EditorStyles.toolbarButton)) return;

            CreateTreeView();
            ReloadFixtures();

            _headerState.ResetSearch();

            _treeView.SetFixtures(_fixtures);
            _treeView.Reload(); // Reload で BuildRoot, BuildRows を実行

            EditorApplication.delayCall += () => _treeView.searchString = FixtureTreeView.DefaultSearchString;
        }

        /// <summary> TreeViewの更新 </summary>
        private void CreateTreeView()
        {
            if (_treeView != null) { return; }

            if (_isCreatingTreeView) { return; }

            _isCreatingTreeView = true;
            Repaint();

            EditorApplication.delayCall += () =>
            {
                if (_columnSearchStates == null || _columnSearchStates.Length != HeaderConfig.HeaderColumnNum)
                {
                    _columnSearchStates = new SearchState[HeaderConfig.HeaderColumnNum];
                    for (var i = 0; i < HeaderConfig.HeaderColumnNum; i++)
                    {
                        _columnSearchStates[i] = new SearchState();
                    }
                }


                _treeViewState ??= new FixtureTreeViewState();
                _headerState ??= new FixtureColumnHeaderState(HeaderConfig.HeaderColumns, _columnSearchStates);
                _headerState.ResetSearch();

                // TreeView作成
                _treeView ??= new FixtureTreeView(_treeViewState, _headerState);
                _treeView.searchString = FixtureTreeView.DefaultSearchString;
                _treeView.Reload(); // Reloadを呼ぶとBuildRootが実行され、次にBuildRowsが実行されます。

                // SearchFieldを初期化
                _searchField ??= new SearchField();
                _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;

                _isCreatingTreeView = false;
            };
        }

        /// <summary> Scene 上から IDmxFixture の GameObject 一覧を取得する </summary>
        private void ReloadFixtures()
        {
            if (_isLoadingFixtures) { return; }

            _isLoadingFixtures = true;

            _fixtures = FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Select(gameObject => (gameObject, fixture: gameObject.GetComponent<IDmxFixture>()))
                .Where(tuple => tuple.fixture != null).ToArray();


            _isLoadingFixtures = false;
        }
    }
}
