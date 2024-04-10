using System.Collections.Generic;
using StageLight.DmxFixture.Channels;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StageLight
{
    public class ChannelListView : VisualElement
    {
        public ChannelListView(List<DmxChannel> channels)
        {
            var baseElement = new VisualElement();
            Add(baseElement);

            const int itemHeight = 48;

            var listView = new ListView(channels, itemHeight, MakeItem, BindItem)
            {
                headerTitle = "Channels",
                selectionType = SelectionType.Single,
                focusable = true,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                showAddRemoveFooter = true,
                showFoldoutHeader = true,
                showBorder = true,
                style =
                {
                    flexGrow = 1.0f
                }
            };

            Add(listView);
            return;

            void BindItem(VisualElement element, int index)
            {
                var channel = channels[index];

                var nameLabel = element.Q<Label>("name");
                nameLabel.text = channel == null ? "Empty" : channel.ChannelName;

                var currentChannelNumber = 1;
                for (var n = 0; n < index; n++)
                {
                    var c = channels[n];
                    currentChannelNumber += c == null ? 1 : c.ChannelSize;
                }

                var channelSize = channel == null ? 1 : channel.ChannelSize;
                var channelLabel = element.Q<Label>("channel");
                channelLabel.text = channelSize == 1
                    ? $"Channel: {currentChannelNumber}"
                    : $"Channel: {currentChannelNumber}-{currentChannelNumber + channelSize - 1}";

                var channelBaseField = element.Q<ObjectField>();
                channelBaseField.value = channel;

                channelBaseField.RegisterValueChangedCallback(evt =>
                {
                    var newChannel = evt.newValue as DmxChannel;
                    channels[index] = newChannel;
                });
            }
        }

        private static VisualElement MakeItem()
        {
            var rootElement = new VisualElement();
            var nameLabel = new Label("Channel Name")
            {
                name = "name",
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold, marginTop = 3, fontSize = 13
                }
            };
            rootElement.Add(nameLabel);

            var channelElement = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row, alignItems = Align.Center,
                }
            };

            var channelLabel = new Label("Channel: 000-000")
            {
                name = "channel",
                style =
                {
                    minWidth = 125
                }
            };
            channelElement.Add(channelLabel);

            var channelBaseField = new ObjectField
            {
                objectType = typeof(DmxChannel),
                style =
                {
                    flexShrink = 1
                }
            };
            channelElement.Add(channelBaseField);
            rootElement.Add(channelElement);

            return rootElement;
        }
    }
}
