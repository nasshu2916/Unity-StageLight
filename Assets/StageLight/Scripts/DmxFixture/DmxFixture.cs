using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StageLight.DmxFixture.Channels;

namespace StageLight.DmxFixture
{
    public class DmxFixture : MonoBehaviour, IDmxFixture
    {
        [SerializeField] private string _name;
        [SerializeField, Range(1, 512)] private int _universe = 1;
        [SerializeField, Range(1, 512)] private int _startAddress = 1;
        [SerializeField] private List<DmxChannel> _channels = new();

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

        public List<DmxChannel> Channels => _channels;

        public int ChannelCount()
        {
            return _channels.Sum(ChannelLength);
        }

        public void UpdateValues(ReadOnlySpan<byte> values)
        {
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
