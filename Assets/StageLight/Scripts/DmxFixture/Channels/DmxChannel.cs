using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace StageLight.DmxFixture.Channels
{
    public abstract class DmxChannel : MonoBehaviour
    {
        [SerializeField] private string _channelName;

        protected DmxChannel()
        {
            var className = GetType().Name;
            _channelName = Regex.Replace(className, "Channel$", "");
        }

        public string ChannelName => _channelName;

        public abstract int ChannelSize { get; }

        public abstract void UpdateValues(ReadOnlySpan<byte> values);
    }
}
