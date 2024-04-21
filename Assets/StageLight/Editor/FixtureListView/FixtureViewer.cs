using System;
using System.Linq;
using StageLight.DmxFixture;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace StageLight.FixtureListView
{
    public class FixtureViewer : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        private bool _isLoadingFixtures;
        private (GameObject gameObject, IDmxFixture fixture)[] _fixtures; // フィクスチャ一覧

        private SearchState[] _columnSearchStates = Array.Empty<SearchState>(); // 検索状態 (列)

        private SearchField _searchField; // 検索窓
        private FixtureTreeView _treeView; // TreeView
        private FixtureTreeViewState _treeViewState; // TreeViewの状態
        private FixtureColumnHeaderState _headerState; // TreeViewヘッダー状態

        [MenuItem("ArtNet/FixtureViewer")]
        public static void ShowWindow()
        {
            var window = GetWindow<FixtureViewer>();
            window.titleContent = new GUIContent("FixtureViewer");
            var position = window.position;
            position.width = HeaderConfig.InitialHeaderTotalWidth + 50f;
            position.height = 400f;
            position.xMin = 400f;
            position.yMin = 100f;
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            VisualElement container = _visualTreeAsset.Instantiate();
            var reloadButton = container.Q<Button>("reloadButton");
            reloadButton.clickable.clicked += ReloadFixtures;

            root.Add(container);

            InitCreateTreeView();
            ReloadFixtures();

            var imguiContainer = new IMGUIContainer(() =>
            {
                var rect = EditorGUILayout.GetControlRect(false, GUILayout.ExpandHeight(true));
                _treeView.OnGUI(rect);
            });
            imguiContainer.style.flexGrow = 1.0f;
            root.Add(imguiContainer);
        }

        private void InitCreateTreeView()
        {
            if (_columnSearchStates == null || _columnSearchStates.Length != HeaderConfig.HeaderColumnNum)
            {
                _columnSearchStates = new SearchState[HeaderConfig.HeaderColumnNum];
                for (var i = 0; i < HeaderConfig.HeaderColumnNum; i++)
                {
                    _columnSearchStates[i] = new SearchState();
                }
            }

            _treeViewState = new FixtureTreeViewState();
            _headerState = new FixtureColumnHeaderState(HeaderConfig.HeaderColumns, _columnSearchStates);
            _headerState.ResetSearch();

            // TreeView作成
            _treeView ??= new FixtureTreeView(_treeViewState, _headerState);
            _treeView.searchString = FixtureTreeView.DefaultSearchString;
            _treeView.Reload(); // Reloadを呼ぶとBuildRootが実行され、次にBuildRowsが実行されます。

            // SearchFieldを初期化
            _searchField ??= new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
        }

        /// <summary> Scene 上から IDmxFixture の GameObject 一覧を取得する </summary>
        private void ReloadFixtures()
        {
            if (_isLoadingFixtures) { return; }

            _isLoadingFixtures = true;

            _fixtures = FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Select(gameObject => (gameObject, fixture: gameObject.GetComponent<IDmxFixture>()))
                .Where(tuple => tuple.fixture != null).ToArray();

            _headerState.ResetSearch();
            _treeView.SetFixtures(_fixtures);
            _treeView.Reload(); // Reload で BuildRoot, BuildRows を実行

            _isLoadingFixtures = false;
        }
    }
}
