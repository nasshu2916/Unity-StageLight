using System;
using UnityEngine;

namespace StageLight.DmxFixture.Channels
{
    [RequireComponent(typeof(LightChannelManager))]
    public class ColorChannel : ChannelBase<Color32>
    {
        [SerializeField] private LightChannelManager _lightChannelManager;

        public override int ChannelSize { get; } = 3;

        private void Reset()
        {
            _lightChannelManager = GetComponent<LightChannelManager>();
        }

        protected override Color32 CastValue(ReadOnlySpan<byte> values)
        {
            return new Color32(values[0], values[1], values[2], 255);
        }

        protected override void UpdateChannel(Color32 value)
        {
            if (_lightChannelManager == null)
            {
                Debug.LogError("LightChannelManager is not set.");
                return;
            }

            _lightChannelManager.Color32 = value;
        }
    }
}
