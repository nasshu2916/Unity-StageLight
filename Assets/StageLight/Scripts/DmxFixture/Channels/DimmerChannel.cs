using System;
using UnityEngine;

namespace StageLight.DmxFixture.Channels
{
    [RequireComponent(typeof(LightChannelManager))]
    public sealed class DimmerChannel : ChannelBase<float>
    {
        [SerializeField] private LightChannelManager _lightChannelManager;

        public override int ChannelSize { get; } = 1;

        public void Reset()
        {
            _lightChannelManager = GetComponent<LightChannelManager>();
        }

        protected override float CastValue(ReadOnlySpan<byte> values)
        {
            return values[0] / 255f;
        }

        protected override void UpdateChannel(float value)
        {
            if (_lightChannelManager == null)
            {
                Debug.LogError("LightChannelManager is not set.");
                return;
            }

            _lightChannelManager.Intensity = value;
        }
    }
}
