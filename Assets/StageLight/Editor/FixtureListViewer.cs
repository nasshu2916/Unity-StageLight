using System;
using System.Collections.Generic;
using System.Linq;
using StageLight.DmxFixture;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace StageLight
{
    public class FixtureListViewer : EditorWindow
    {
        private List<IDmxFixture> _fixtures = new();
        private MultiColumnListView _multiColumnListView;

        [MenuItem("ArtNet/FixtureList")]
        public static void ShowFixtureListViewer()
        {
            var wnd = GetWindow<FixtureListViewer>();
            wnd.titleContent = new GUIContent("ChannelListViewer");
        }

        public void CreateGUI()
        {
            _fixtures = FindAllFixtures();
            var root = rootVisualElement;

            var headerElement = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row, alignItems = Align.Center
                }
            };
            headerElement.Add(new Button(Refresh)
            {
                text = "Reload",
                style =
                {
                    width = 100
                }
            });
            headerElement.Add(new Label("Reloads the list of fixtures"));

            root.Add(headerElement);

            _multiColumnListView = new MultiColumnListView
            {
                itemsSource = _fixtures
            };

            var objectNameColumn = new Column
            {
                title = "Object Name",
                width = 100,
                bindCell = (e, i) =>
                {
                    e.Clear();
                    var obj = (Object)_fixtures[i];
                    var label = new Label
                    {
                        text = obj != null ? obj.name : "",
                        style =
                        {
                            height = Length.Percent(100)
                        }
                    };

                    label.RegisterCallback<ClickEvent>(evt =>
                    {
                        EditorGUIUtility.PingObject(obj);
                    });
                    e.Add(label);
                }
            };
            _multiColumnListView.columns.Add(objectNameColumn);

            var nameColumn = new Column
            {
                title = "Fixture Name", width = 100, bindCell = (e, i) => e.Q<Label>().text = _fixtures[i].Name
            };
            _multiColumnListView.columns.Add(nameColumn);

            var universeColumn = new Column
            {
                title = "Universe",
                width = 100,
                bindCell = (e, i) =>
                {
                    e.Clear();
                    var field = new IntegerField
                    {
                        value = _fixtures[i].Universe
                    };
                    field.RegisterValueChangedCallback(evt =>
                    {
                        var value = evt.newValue;
                        if (value is <= 0 or > 512) return;

                        UpdateFixture(i, f => f.Universe = value);
                    });
                    e.Add(field);
                }
            };
            _multiColumnListView.columns.Add(universeColumn);

            var startAddressColumn = new Column
            {
                title = "Start Address",
                width = 100,
                bindCell = (e, i) =>
                {
                    e.Clear();
                    var field = new IntegerField
                    {
                        value = _fixtures[i].StartAddress
                    };
                    field.RegisterValueChangedCallback(evt =>
                    {
                        var value = evt.newValue;
                        if (value is <= 0 or > 512) return;

                        UpdateFixture(i, f => f.StartAddress = value);
                    });
                    e.Add(field);
                }
            };
            _multiColumnListView.columns.Add(startAddressColumn);

            var channelCountColumn = new Column
            {
                title = "Channel Count",
                width = 100,
                bindCell = (e, i) => e.Q<Label>().text = _fixtures[i].ChannelCount().ToString()
            };
            _multiColumnListView.columns.Add(channelCountColumn);

            root.Add(_multiColumnListView);
        }

        private void Refresh()
        {
            _fixtures = FindAllFixtures();
            if (_multiColumnListView != null) _multiColumnListView.itemsSource = _fixtures;
        }

        private static List<IDmxFixture> FindAllFixtures()
        {
            return FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Select(gameObject => gameObject.GetComponent<IDmxFixture>()).Where(fixture => fixture != null)
                .OrderBy(fixture => fixture.Name).ThenBy(fixture => ((Object)fixture).name).ToList();
        }

        private void UpdateFixture(int index, Action<IDmxFixture> updateFun)
        {
            updateFun(_fixtures[index]);
            EditorUtility.SetDirty((Object)_fixtures[index]);
        }
    }
}
