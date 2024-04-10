using System;

namespace StageLight.DmxFixture
{
    public interface IDmxFixture
    {
        public string Name { get; set; }
        public int Universe { get; set; }
        public int StartAddress { get; set; }
        public int ChannelMode { get; }

        public void UpdateValues(ReadOnlySpan<byte> values);
    }
}
