using System;
using System.Collections.Generic;
using System.Linq;
using StageLight.DmxFixture.Channels;
using UnityEngine;

namespace StageLight.DmxFixture
{
    public class DmxFixture : MonoBehaviour, IDmxFixture
    {
        [SerializeField] private string _name;
        [SerializeField, Range(1, 512)] private int _universe = 1;
        [SerializeField, Range(1, 512)] private int _startAddress = 1;
        [SerializeField] private List<DmxChannel> _channels = new();
        [SerializeField] private int _channelMode = 1;

        public List<DmxChannel> Channels => _channels;

        private void Reset()
        {
            if (string.IsNullOrEmpty(_name))
            {
                _name = name;
            }
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int Universe
        {
            get => _universe;
            set => _universe = value;
        }

        public int StartAddress
        {
            get => _startAddress;
            set => _startAddress = value;
        }

        public int ChannelMode => _channelMode;

        public void UpdateValues(ReadOnlySpan<byte> values)
        {
            if (_channels.Sum(ChannelLength) > ChannelMode)
            {
                Debug.LogWarning("Total Channel size exceeds ChannelMode");
                return;
            }

            var index = 0;
            foreach (var channel in _channels)
            {
                var channelSize = ChannelLength(channel);
                if (channel != null)
                {
                    channel.UpdateValues(values.Slice(index, channelSize));
                }

                index += channelSize;
            }
        }

        private static int ChannelLength(DmxChannel channel)
        {
            return channel != null ? channel.ChannelSize : 1;
        }
    }
}
