using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StageLight
{
    [CustomEditor(typeof(DmxFixture.DmxFixture))]
    public class DmxFixtureEditor : Editor
    {
        private DmxFixture.DmxFixture _targetDmxFixture;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            _targetDmxFixture = (DmxFixture.DmxFixture)target;
            if (_targetDmxFixture == null) return root;

            var nameField = new PropertyField(serializedObject.FindProperty("_name"));
            root.Add(nameField);

            var universeField = new PropertyField(serializedObject.FindProperty("_universe"));
            root.Add(universeField);

            var startAddressField = new PropertyField(serializedObject.FindProperty("_startAddress"));
            root.Add(startAddressField);

            var channelListView = new ChannelListView(_targetDmxFixture!.Channels);
            root.Add(channelListView);

            var channelModeField = new PropertyField(serializedObject.FindProperty("_channelMode"));
            root.Add(channelModeField);

            return root;
        }
    }
}
